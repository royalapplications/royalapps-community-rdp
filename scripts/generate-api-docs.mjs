import { execFileSync } from "node:child_process";
import { mkdirSync, readdirSync, readFileSync, rmSync, writeFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const repoRoot = path.resolve(__dirname, "..");
const docsRoot = path.join(repoRoot, "docs");
const apiRoot = path.join(docsRoot, "api");
const generatedRoot = path.join(apiRoot, "reference");
const sidebarFile = path.join(apiRoot, "sidebar.mjs");
const projectFile = path.join(
  repoRoot,
  "src",
  "RoyalApps.Community.Rdp.WinForms",
  "RoyalApps.Community.Rdp.WinForms.csproj");
const xmlDocFile = path.join(
  repoRoot,
  "src",
  "RoyalApps.Community.Rdp.WinForms",
  "bin",
  "Release",
  "net10.0-windows",
  "RoyalApps.Community.Rdp.WinForms.xml");
const documentedNamespacePrefix = "RoyalApps.Community.Rdp.WinForms";
const publicApi = collectPublicApiSurface(path.join(repoRoot, "src", "RoyalApps.Community.Rdp.WinForms"));

execFileSync(
  "dotnet",
  ["build", projectFile, "-c", "Release", "-nologo", "-p:UseSharedCompilation=false", "/p:EnableWindowsTargeting=true"],
  { cwd: repoRoot, stdio: "inherit" });

mkdirSync(generatedRoot, { recursive: true });
for (const entry of readdirSync(generatedRoot, { withFileTypes: true })) {
  rmSync(path.join(generatedRoot, entry.name), { recursive: true, force: true });
}

const xml = readFileSync(xmlDocFile, "utf8");
const members = Array.from(xml.matchAll(/<member name="([^"]+)">([\s\S]*?)<\/member>/g)).map(match => ({
  id: match[1],
  body: match[2]
}));

const memberDocs = new Map(members.map(member => [member.id, parseMemberBody(member.id, member.body)]));
const resolvedMemberDocs = new Map();
const typeMembers = members.filter(member =>
  member.id.startsWith("T:") &&
  publicApi.publicTypes.has(member.id.slice(2)));
const pagesByNamespace = new Map();

for (const member of members) {
  getResolvedDocs(member.id);
}

for (const typeMember of typeMembers) {
  const fullTypeName = typeMember.id.slice(2);
  const namespaceName = fullTypeName.slice(0, fullTypeName.lastIndexOf("."));
  const typeName = fullTypeName.slice(fullTypeName.lastIndexOf(".") + 1);
  const simpleTypeName = typeName.split("`")[0];
  const slug = slugify(fullTypeName);
  const linkedMembers = members
    .filter(member =>
      member.id !== typeMember.id &&
      getOwningType(member.id) === fullTypeName &&
      isDocumentedMember(fullTypeName, member.id))
    .sort((left, right) => compareMembers(left, right, publicApi.enumValueOrder));

  const pagePath = path.join(generatedRoot, `${slug}.md`);
  writeFileSync(pagePath, renderTypePage(fullTypeName, simpleTypeName, typeMember.id, linkedMembers, resolvedMemberDocs, publicApi));

  const entries = pagesByNamespace.get(namespaceName) ?? [];
  entries.push({ fullTypeName, simpleTypeName, slug });
  pagesByNamespace.set(namespaceName, entries);
}

for (const entries of pagesByNamespace.values()) {
  entries.sort(compareTypeEntries);
}

const orderedNamespaces = Array.from(pagesByNamespace.keys()).sort((left, right) => left.localeCompare(right));
writeFileSync(path.join(apiRoot, "index.md"), renderApiIndex());
writeFileSync(sidebarFile, renderSidebarModule(orderedNamespaces, pagesByNamespace));

function parseMemberBody(memberId, body) {
  const inheritdocMatch = body.match(/<inheritdoc(?:\s+cref="([^"]+)")?\s*\/>/);
  return {
    summary: cleanupXml(extractTag(body, "summary")),
    remarks: cleanupXml(extractTag(body, "remarks")),
    returns: cleanupXml(extractTag(body, "returns")),
    externalLinks: extractExternalLinks(body),
    inheritdocCref: inheritdocMatch?.[1]
      ? resolveCref(memberId, inheritdocMatch[1])
      : "",
    hasInheritdoc: Boolean(inheritdocMatch),
    params: Array.from(body.matchAll(/<param name="([^"]+)">([\s\S]*?)<\/param>/g)).map(match => ({
      name: match[1],
      description: cleanupXml(match[2])
    }))
  };
}

function extractTag(body, tagName) {
  const match = body.match(new RegExp(`<${tagName}>([\\s\\S]*?)<\\/${tagName}>`));
  return match ? match[1] : "";
}

function extractExternalLinks(body) {
  const links = new Set();

  for (const match of body.matchAll(/cref="(https?:\/\/[^"]+)"/g)) {
    links.add(match[1]);
  }

  for (const match of body.matchAll(/<cref>(https?:\/\/[\s\S]*?)<\/cref>/g)) {
    links.add(match[1].trim());
  }

  return Array.from(links);
}

function cleanupXml(text) {
  return text
    .replace(/<see\s+langword="([^"]+)"\s*\/>/g, "`$1`")
    .replace(/<paramref\s+name="([^"]+)"\s*\/>/g, "`$1`")
    .replace(/<see\s+cref="([^"]+)"\s*\/>/g, (_, cref) => renderCref(cref))
    .replace(/<c>([\s\S]*?)<\/c>/g, "`$1`")
    .replace(/<\/?para>/g, "\n\n")
    .replace(/<\/?[^>]+>/g, "")
    .replace(/&lt;/g, "<")
    .replace(/&gt;/g, ">")
    .replace(/&quot;/g, "\"")
    .replace(/&apos;/g, "'")
    .replace(/&amp;/g, "&")
    .split(/\r?\n/)
    .map(line => line.trim())
    .filter((line, index, lines) => line.length > 0 || (index > 0 && lines[index - 1].length > 0))
    .join("\n");
}

function formatCref(cref) {
  const value = cref.includes(":") ? cref.slice(cref.indexOf(":") + 1) : cref;
  return value.replace(/`[0-9]+/g, "");
}

function renderCref(cref) {
  if (!cref) {
    return "";
  }

  if (/^https?:\/\//i.test(cref)) {
    return `[${cref}](${cref})`;
  }

  const normalizedCref = cref.includes(":") ? cref : `T:${cref}`;
  const kind = normalizedCref[0];
  const formatted = formatCref(normalizedCref);

  if (kind === "T" && publicApi.publicTypes.has(formatted)) {
    return `[${getDisplayTypeName(formatted)}](${getTypeDocLink(formatted)})`;
  }

  if (kind !== "T") {
    const owningType = getOwningType(normalizedCref);
    if (publicApi.publicTypes.has(owningType)) {
      const label = `${getDisplayTypeName(owningType)}.${getMemberName(normalizedCref)}`;
      return `[${label}](${getTypeDocLink(owningType)})`;
    }
  }

  return `\`${formatted}\``;
}

function resolveCref(memberId, cref) {
  if (!cref) {
    return "";
  }

  if (cref.includes(":")) {
    return cref;
  }

  const memberKind = memberId[0];
  if (memberKind === "T") {
    const typeInfo = publicApi.typeInfos.get(memberId.slice(2));
    const resolvedType = resolveTypeReference(
      cref,
      typeInfo?.namespaceName ?? "",
      publicApi.simpleTypeLookup,
      publicApi.publicTypes);
    return resolvedType ? `T:${resolvedType}` : cref;
  }

  const owningType = getOwningType(memberId);
  if (cref.includes(".")) {
    const splitIndex = cref.lastIndexOf(".");
    const typeReference = cref.slice(0, splitIndex);
    const memberName = cref.slice(splitIndex + 1);
    const resolvedType = resolveTypeReference(
      typeReference,
      owningType.slice(0, owningType.lastIndexOf(".")),
      publicApi.simpleTypeLookup,
      publicApi.publicTypes);
    return resolvedType ? `${memberKind}:${resolvedType}.${memberName}` : `${memberKind}:${cref}`;
  }

  const inheritedMemberCref = resolveInheritedMemberCref(memberId, cref);
  return inheritedMemberCref || `${memberKind}:${owningType}.${cref}`;
}

function getOwningType(memberId) {
  const value = memberId.slice(2);
  const signatureStart = value.indexOf("(");
  const withoutSignature = signatureStart >= 0 ? value.slice(0, signatureStart) : value;
  const lastDot = withoutSignature.lastIndexOf(".");
  return lastDot >= 0 ? withoutSignature.slice(0, lastDot) : withoutSignature;
}

function compareMembers(left, right, enumValueOrder) {
  const leftKind = memberSortOrder(left.id[0]);
  const rightKind = memberSortOrder(right.id[0]);
  if (leftKind !== rightKind) {
    return leftKind - rightKind;
  }

  if (left.id[0] === "F") {
    const owningType = getOwningType(left.id);
    const order = enumValueOrder.get(owningType);
    if (order) {
      return (order.get(getMemberName(left.id)) ?? Number.MAX_SAFE_INTEGER) -
        (order.get(getMemberName(right.id)) ?? Number.MAX_SAFE_INTEGER);
    }
  }

  return left.id.localeCompare(right.id);
}

function memberSortOrder(kind) {
  switch (kind) {
    case "P":
      return 0;
    case "E":
      return 1;
    case "M":
      return 2;
    case "F":
      return 3;
    default:
      return 4;
  }
}

function renderTypePage(fullTypeName, simpleTypeName, typeMemberId, linkedMembers, docsByMember, publicApiSurface) {
  const typeDocs = docsByMember.get(typeMemberId) ?? emptyDocs();
  const typeInfo = publicApiSurface.typeInfos.get(fullTypeName);
  const grouped = new Map();

  for (const member of linkedMembers) {
    const kind = memberHeading(member.id[0], typeInfo?.kind);
    const items = grouped.get(kind) ?? [];
    items.push(member);
    grouped.set(kind, items);
  }

  const lines = [
    `# \`${simpleTypeName}\``,
    "",
    typeDocs.summary || "No summary available.",
    "",
    "## Type",
    "",
    "```csharp",
    fullTypeName,
    "```"
  ];

  const relatedTypes = (typeInfo?.relatedTypes ?? []).filter(relatedType => publicApiSurface.publicTypes.has(relatedType));
  if (relatedTypes.length > 0) {
    lines.push("", "## Related Types", "");
    for (const relatedType of relatedTypes) {
      lines.push(`- ${renderTypeLink(relatedType)}`);
    }
  }

  const usedByTypes = (publicApiSurface.reverseTypeUsages.get(fullTypeName) ?? [])
    .filter(usedByType => usedByType !== fullTypeName);
  if (usedByTypes.length > 0) {
    lines.push("", "## Used By", "");
    for (const usedByType of usedByTypes) {
      lines.push(`- ${renderTypeLink(usedByType)}`);
    }
  }

  if (typeDocs.remarks) {
    lines.push("", "## Remarks", "", typeDocs.remarks);
  }

  for (const heading of ["Properties", "Events", "Methods", "Values", "Fields"]) {
    const items = grouped.get(heading);
    if (!items || items.length === 0) {
      continue;
    }

    lines.push("", `## ${heading}`);
    for (const item of items) {
      const itemDocs = docsByMember.get(item.id) ?? emptyDocs();
      const itemMetadata = getMemberMetadata(fullTypeName, item.id, publicApiSurface);
      lines.push("", `### ${formatMemberHeading(item.id, itemMetadata, itemDocs, publicApiSurface)}`, "");

      if (heading === "Events") {
        if (itemMetadata?.type) {
          lines.push("", `Handler Type: ${renderTypeExpression(itemMetadata.type, publicApiSurface)}`);
        }
      }

      if (heading === "Methods") {
        if (itemMetadata?.returnType) {
          const returnText = itemDocs.returns
            ? `${renderTypeExpression(itemMetadata.returnType, publicApiSurface)}. ${itemDocs.returns}`
            : renderTypeExpression(itemMetadata.returnType, publicApiSurface);
          lines.push("", `Returns: ${returnText}`);
        }
      }

      lines.push("", itemDocs.summary || "No summary available.");

      if (itemDocs.params.length > 0 || itemMetadata?.parameters?.length > 0) {
        const parameterDocs = new Map(itemDocs.params.map(parameter => [parameter.name, parameter.description]));
        lines.push("", "**Parameters**", "");
        for (const parameter of itemMetadata?.parameters ?? itemDocs.params) {
          const description = parameterDocs.get(parameter.name);
          const typeText = parameter.type
            ? ` (${renderTypeExpression(parameter.type, publicApiSurface)})`
            : "";
          lines.push(`- \`${parameter.name}\`${typeText}: ${description ?? "No description available."}`);
        }
      }

      if (itemDocs.remarks) {
        lines.push("", itemDocs.remarks);
      }
    }
  }

  lines.push("", "[Back to API index](../index.md)");
  return `${lines.join("\n")}\n`;
}

function memberHeading(kind, typeKind) {
  if (kind === "F" && typeKind === "enum") {
    return "Values";
  }

  switch (kind) {
    case "P":
      return "Properties";
    case "E":
      return "Events";
    case "F":
      return "Fields";
    default:
      return "Methods";
  }
}

function formatMemberSignature(memberId, publicApiSurface) {
  const kind = memberId[0];
  const value = memberId.slice(2);
  const signatureStart = value.indexOf("(");
  const withoutSignature = signatureStart >= 0 ? value.slice(0, signatureStart) : value;
  const parameters = signatureStart >= 0
    ? value.slice(signatureStart + 1, value.lastIndexOf(")"))
    : "";
  const memberName = withoutSignature.slice(withoutSignature.lastIndexOf(".") + 1);

  if (kind === "F") {
    const owningType = getOwningType(memberId);
    const enumValues = publicApiSurface.enumValues.get(owningType);
    const memberName = withoutSignature.slice(withoutSignature.lastIndexOf(".") + 1);
    const enumValue = enumValues?.get(memberName);
    return enumValue === undefined ? memberName : `${memberName} = ${enumValue}`;
  }

  if (kind === "P" || kind === "E") {
    return memberName;
  }

  const displayName = memberName === "#ctor" ? "Constructor" : memberName;
  const parameterList = parameters.length === 0
    ? ""
    : parameters
      .split(",")
      .filter(Boolean)
      .map(parameter => simplifyTypeName(parameter.trim()))
      .join(", ");
  return `${displayName}(${parameterList})`;
}

function formatMemberHeading(memberId, itemMetadata, itemDocs, publicApiSurface) {
  const signature = `\`${formatMemberSignature(memberId, publicApiSurface)}\``;
  const externalLink = getPrimaryExternalLink(memberId, itemDocs);
  const externalGlyph = externalLink
    ? ` <a class="api-member-external-link" href="${externalLink}" target="_blank" rel="noreferrer" title="Open external reference">↗</a>`
    : "";

  if ((memberId[0] === "P" || memberId[0] === "F") && itemMetadata?.type) {
    return `${signature} (${renderTypeExpression(itemMetadata.type, publicApiSurface)})${externalGlyph}`;
  }

  return `${signature}${externalGlyph}`;
}

function getPrimaryExternalLink(memberId, itemDocs) {
  if (memberId[0] !== "P" && memberId[0] !== "M") {
    return "";
  }

  return itemDocs.externalLinks[0] ?? "";
}

function simplifyTypeName(typeName) {
  return typeName
    .replace(/System\./g, "")
    .replace(/Microsoft\.Extensions\.Logging\./g, "")
    .replace(/RoyalApps\.Community\.Rdp\.WinForms\./g, "")
    .replace(/\{/g, "<")
    .replace(/\}/g, ">");
}

function slugify(value) {
  return value.replace(/[^A-Za-z0-9]+/g, "-").replace(/^-+|-+$/g, "").toLowerCase();
}

function getTypeDocLink(fullTypeName) {
  return `/api/reference/${slugify(fullTypeName)}`;
}

function getDisplayTypeName(fullTypeName) {
  return fullTypeName.slice(fullTypeName.lastIndexOf(".") + 1).replace(/`[0-9]+/g, "");
}

function renderTypeLink(fullTypeName) {
  return `[${getDisplayTypeName(fullTypeName)}](${getTypeDocLink(fullTypeName)})`;
}

function renderApiIndex() {
  return "# API Reference\n\nUse the sidebar to browse namespaces and types.\n";
}

function renderSidebarModule(namespaces, pagesByNamespace) {
  const groups = [];

  const topLevelSpecs = [
    { text: "Rdp.WinForms", namespaces: [documentedNamespacePrefix] },
    { text: "Clients", namespaces: [`${documentedNamespacePrefix}.Clients`] },
    { text: "Events", namespaces: [`${documentedNamespacePrefix}.Events`] },
    {
      text: "Configuration",
      sections: [
        { text: "Core", namespaces: [`${documentedNamespacePrefix}.Configuration`] },
        { text: "Connection", namespaces: [`${documentedNamespacePrefix}.Configuration.Connection`] },
        { text: "Display", namespaces: [`${documentedNamespacePrefix}.Configuration.Display`] },
        { text: "Input", namespaces: [`${documentedNamespacePrefix}.Configuration.Input`] },
        { text: "Performance", namespaces: [`${documentedNamespacePrefix}.Configuration.Performance`] },
        { text: "Applications", namespaces: [`${documentedNamespacePrefix}.Configuration.Applications`] },
        { text: "Redirection", namespaces: [`${documentedNamespacePrefix}.Configuration.Redirection`] },
        { text: "Security", namespaces: [`${documentedNamespacePrefix}.Configuration.Security`] },
        { text: "External", namespaces: [`${documentedNamespacePrefix}.Configuration.External`] }
      ]
    },
    { text: "RDP Files", namespaces: [`${documentedNamespacePrefix}.External.Files`] }
  ];

  for (const spec of topLevelSpecs) {
    const group = createSidebarGroup(spec, pagesByNamespace);
    if (group) {
      groups.push(group);
    }
  }

  for (const namespaceName of namespaces) {
    if (isKnownSidebarNamespace(namespaceName)) {
      continue;
    }

    const items = renderSidebarItems([namespaceName], pagesByNamespace);
    if (items.length === 0) {
      continue;
    }

    groups.push({
      text: formatNamespaceLabel(namespaceName),
      collapsed: false,
      items
    });
  }

  return `export default ${JSON.stringify(groups, null, 2)};\n`;
}

function createSidebarGroup(spec, pagesByNamespace) {
  if (spec.sections) {
    const items = spec.sections
      .map(section => createSidebarSection(section, pagesByNamespace))
      .filter(Boolean);

    if (items.length === 0) {
      return null;
    }

    return {
      text: spec.text,
      collapsed: false,
      items
    };
  }

  const items = renderSidebarItems(spec.namespaces ?? [], pagesByNamespace);
  if (items.length === 0) {
    return null;
  }

  return {
    text: spec.text,
    collapsed: false,
    items
  };
}

function createSidebarSection(section, pagesByNamespace) {
  const items = renderSidebarItems(section.namespaces ?? [], pagesByNamespace);
  if (items.length === 0) {
    return null;
  }

  return {
    text: section.text,
    collapsed: false,
    items
  };
}

function renderSidebarItems(namespaces, pagesByNamespace) {
  const items = [];

  for (const namespaceName of namespaces) {
    for (const entry of pagesByNamespace.get(namespaceName) ?? []) {
      items.push({
        text: entry.simpleTypeName,
        link: `/api/reference/${entry.slug}`
      });
    }
  }

  return items;
}

function isKnownSidebarNamespace(namespaceName) {
  const knownNamespaces = new Set([
    documentedNamespacePrefix,
    `${documentedNamespacePrefix}.Clients`,
    `${documentedNamespacePrefix}.Events`,
    `${documentedNamespacePrefix}.Configuration`,
    `${documentedNamespacePrefix}.Configuration.Connection`,
    `${documentedNamespacePrefix}.Configuration.Display`,
    `${documentedNamespacePrefix}.Configuration.Input`,
    `${documentedNamespacePrefix}.Configuration.Performance`,
    `${documentedNamespacePrefix}.Configuration.Applications`,
    `${documentedNamespacePrefix}.Configuration.Redirection`,
    `${documentedNamespacePrefix}.Configuration.Security`,
    `${documentedNamespacePrefix}.Configuration.External`,
    `${documentedNamespacePrefix}.External.Files`
  ]);

  return knownNamespaces.has(namespaceName);
}

function compareTypeEntries(left, right) {
  const leftMatch = left.simpleTypeName.match(/^RdpClient(\d+)$/);
  const rightMatch = right.simpleTypeName.match(/^RdpClient(\d+)$/);
  if (leftMatch && rightMatch) {
    return Number(rightMatch[1]) - Number(leftMatch[1]);
  }

  return left.simpleTypeName.localeCompare(right.simpleTypeName, undefined, { numeric: true });
}

function formatNamespaceLabel(namespaceName) {
  if (namespaceName === documentedNamespacePrefix) {
    return "Rdp.WinForms";
  }

  return namespaceName.replace(`${documentedNamespacePrefix}.`, "");
}

function emptyDocs() {
  return {
    summary: "",
    remarks: "",
    returns: "",
    externalLinks: [],
    inheritdocCref: "",
    hasInheritdoc: false,
    params: []
  };
}

function isDocumentedMember(fullTypeName, memberId) {
  const typeInfo = publicApi.typeInfos.get(fullTypeName);
  if (typeInfo?.kind === "interface") {
    return true;
  }

  const allowedMembers = publicApi.publicMembers.get(fullTypeName);
  return allowedMembers ? allowedMembers.has(getMemberName(memberId)) : false;
}

function getMemberMetadata(fullTypeName, memberId, publicApiSurface) {
  return publicApiSurface.memberDetails.get(fullTypeName)?.get(getMemberMetadataKey(memberId));
}

function getMemberName(memberId) {
  const value = memberId.slice(2);
  const signatureStart = value.indexOf("(");
  const withoutSignature = signatureStart >= 0 ? value.slice(0, signatureStart) : value;
  return withoutSignature.slice(withoutSignature.lastIndexOf(".") + 1);
}

function getMemberSuffix(memberId) {
  const value = memberId.slice(2);
  const owningType = getOwningType(memberId);
  return value.startsWith(`${owningType}.`)
    ? value.slice(owningType.length + 1)
    : getMemberName(memberId);
}

function getMemberMetadataKey(memberId) {
  if (memberId[0] !== "M") {
    return getMemberName(memberId);
  }

  const suffix = getMemberSuffix(memberId);
  const signatureStart = suffix.indexOf("(");
  const methodName = signatureStart >= 0 ? suffix.slice(0, signatureStart) : suffix;
  const parameterList = signatureStart >= 0
    ? suffix.slice(signatureStart + 1, suffix.lastIndexOf(")"))
    : "";
  const parameterCount = parameterList.length === 0
    ? 0
    : splitTopLevel(parameterList, ",").length;
  return `${methodName}/${parameterCount}`;
}

function collectPublicApiSurface(sourceRoot) {
  const publicTypes = new Set();
  const publicMembers = new Map();
  const memberDetails = new Map();
  const reverseTypeUsages = new Map();
  const typeInfos = new Map();
  const enumValues = new Map();
  const enumValueOrder = new Map();
  const simpleTypeLookup = new Map();

  for (const filePath of enumerateSourceFiles(sourceRoot)) {
    const content = readFileSync(filePath, "utf8");
    const namespaceMatch = content.match(/namespace\s+([A-Za-z0-9_.]+)\s*;/);
    if (!namespaceMatch) {
      continue;
    }

    const namespaceName = namespaceMatch[1];
    const lines = content.split(/\r?\n/);
    let braceDepth = 0;
    let currentType = null;

    for (const line of lines) {
      const trimmed = line.trim();
      const opens = countOccurrences(line, "{");
      const closes = countOccurrences(line, "}");

      const publicTypeMatch = trimmed.match(/^public\s+(?:static\s+|sealed\s+|abstract\s+|partial\s+)*(class|enum|interface|struct)\s+(\w+)(?:\s*:\s*([^{]+))?/);
      if (!currentType && publicTypeMatch) {
        const fullTypeName = `${namespaceName}.${publicTypeMatch[2]}`;
        const kind = publicTypeMatch[1];
        currentType = {
          fullTypeName,
          simpleTypeName: publicTypeMatch[2],
          namespaceName,
          kind,
          declarationDepth: braceDepth + 1,
          awaitingOpenBrace: opens === 0,
          relatedTypeReferences: splitTypeReferences(publicTypeMatch[3]),
          nextEnumValue: 0
        };
        publicTypes.add(fullTypeName);
        publicMembers.set(fullTypeName, new Set());
        memberDetails.set(fullTypeName, new Map());
        typeInfos.set(fullTypeName, {
          fullTypeName,
          simpleTypeName: publicTypeMatch[2],
          namespaceName,
          kind,
          relatedTypeReferences: splitTypeReferences(publicTypeMatch[3]),
          relatedTypes: []
        });
        simpleTypeLookup.set(publicTypeMatch[2], fullTypeName);
        if (kind === "enum") {
          enumValues.set(fullTypeName, new Map());
          enumValueOrder.set(fullTypeName, new Map());
        }
      }
      else if (currentType?.kind === "enum") {
        const enumMember = tryParseEnumMember(trimmed, enumValues.get(currentType.fullTypeName) ?? new Map(), currentType.nextEnumValue);
        if (enumMember) {
          publicMembers.get(currentType.fullTypeName)?.add(enumMember.name);
          enumValues.get(currentType.fullTypeName)?.set(enumMember.name, enumMember.value);
          enumValueOrder.get(currentType.fullTypeName)?.set(enumMember.name, enumValueOrder.get(currentType.fullTypeName)?.size ?? 0);
          currentType.nextEnumValue = enumMember.value + 1;
        }
      }
      else if (currentType && shouldParseMember(trimmed, currentType.kind)) {
        const memberDeclaration = tryParseMemberDeclaration(trimmed, currentType.simpleTypeName);
        if (memberDeclaration) {
          publicMembers.get(currentType.fullTypeName)?.add(memberDeclaration.name);
          memberDetails.get(currentType.fullTypeName)?.set(memberDeclaration.key, memberDeclaration);
        }
      }

      braceDepth += opens;
      if (currentType?.awaitingOpenBrace && braceDepth >= currentType.declarationDepth) {
        currentType.awaitingOpenBrace = false;
      }

      braceDepth -= closes;

      if (currentType && !currentType.awaitingOpenBrace && braceDepth < currentType.declarationDepth) {
        currentType = null;
      }
    }
  }

  for (const typeInfo of typeInfos.values()) {
    typeInfo.relatedTypes = typeInfo.relatedTypeReferences
      .map(reference => resolveTypeReference(reference, typeInfo.namespaceName, simpleTypeLookup, publicTypes))
      .filter(Boolean);
  }

  for (const [declaringType, members] of memberDetails.entries()) {
    const namespaceName = typeInfos.get(declaringType)?.namespaceName ?? "";
    for (const member of members.values()) {
      for (const referencedType of collectReferencedPublicTypes(member, namespaceName, simpleTypeLookup, publicTypes)) {
        if (referencedType === declaringType) {
          continue;
        }

        const usedBy = reverseTypeUsages.get(referencedType) ?? [];
        if (!usedBy.includes(declaringType)) {
          usedBy.push(declaringType);
          usedBy.sort((left, right) => compareTypeEntries(
            { simpleTypeName: getDisplayTypeName(left) },
            { simpleTypeName: getDisplayTypeName(right) }));
          reverseTypeUsages.set(referencedType, usedBy);
        }
      }
    }
  }

  return { publicTypes, publicMembers, memberDetails, reverseTypeUsages, typeInfos, enumValues, enumValueOrder, simpleTypeLookup };
}

function enumerateSourceFiles(directory) {
  const results = [];
  for (const entry of readdirSync(directory, { withFileTypes: true })) {
    if (entry.name === "bin" || entry.name === "obj") {
      continue;
    }

    const fullPath = path.join(directory, entry.name);
    if (entry.isDirectory()) {
      results.push(...enumerateSourceFiles(fullPath));
    }
    else if (entry.isFile() && entry.name.endsWith(".cs")) {
      results.push(fullPath);
    }
  }

  return results;
}

function tryParseMemberDeclaration(trimmedLine, simpleTypeName) {
  if (/^(public|internal|protected|private)\s+(?:static\s+|sealed\s+|abstract\s+|partial\s+)*(class|enum|interface|struct)\s+/.test(trimmedLine)) {
    return null;
  }

  const normalizedLine = trimmedLine
    .replace(/^(public|protected|private)\s+/, "")
    .replace(/^(static|virtual|override|abstract|sealed|partial|new|extern|unsafe)\s+/g, "");

  const eventMatch = normalizedLine.match(/^event\s+(.+?)\s+(\w+)\s*[;{]/);
  if (eventMatch) {
    return {
      key: eventMatch[2],
      kind: "event",
      name: eventMatch[2],
      type: eventMatch[1].trim()
    };
  }

  const constructorMatch = normalizedLine.match(new RegExp(`^${simpleTypeName}\\s*\\(([^)]*)\\)\\s*(?:=>|\\{|;)`));
  if (constructorMatch) {
    const parameters = parseParameters(constructorMatch[1]);
    return {
      key: `#ctor/${parameters.length}`,
      kind: "method",
      name: "#ctor",
      returnType: "",
      parameters
    };
  }

  const methodMatch = normalizedLine.match(/^(.+?)\s+(\w+)\s*\(([^)]*)\)\s*(?:=>|\{|;)/);
  if (methodMatch) {
    const parameters = parseParameters(methodMatch[3]);
    return {
      key: `${methodMatch[2]}/${parameters.length}`,
      kind: "method",
      name: methodMatch[2],
      returnType: methodMatch[1].trim(),
      parameters
    };
  }

  const propertyMatch = normalizedLine.match(/^(.+?)\s+(\w+)\s*(?:=>|\{|;)/);
  if (propertyMatch) {
    return {
      key: propertyMatch[2],
      kind: "property",
      name: propertyMatch[2],
      type: propertyMatch[1].trim()
    };
  }

  return null;
}

function parseParameters(parameterList) {
  if (!parameterList || parameterList.trim().length === 0) {
    return [];
  }

  return splitTopLevel(parameterList, ",")
    .map(parameter => parseParameter(parameter))
    .filter(Boolean);
}

function parseParameter(parameter) {
  const trimmedParameter = parameter.replace(/\s*=\s*.+$/, "").trim();
  if (trimmedParameter.length === 0) {
    return null;
  }

  const parts = trimmedParameter.split(/\s+/);
  const name = parts.pop();
  if (!name) {
    return null;
  }

  return {
    name,
    type: parts.join(" ")
  };
}

function splitTopLevel(text, separator) {
  const parts = [];
  let depth = 0;
  let current = "";

  for (const character of text) {
    if (character === "<" || character === "(" || character === "[" || character === "{") {
      depth += 1;
    }
    else if (character === ">" || character === ")" || character === "]" || character === "}") {
      depth = Math.max(0, depth - 1);
    }

    if (character === separator && depth === 0) {
      parts.push(current.trim());
      current = "";
      continue;
    }

    current += character;
  }

  if (current.trim().length > 0) {
    parts.push(current.trim());
  }

  return parts;
}

function shouldParseMember(trimmedLine, typeKind) {
  if (trimmedLine.length === 0 ||
      trimmedLine.startsWith("//") ||
      trimmedLine.startsWith("[") ||
      trimmedLine === "{" ||
      trimmedLine === "}") {
    return false;
  }

  if (typeKind === "interface") {
    return !trimmedLine.startsWith("internal ");
  }

  return trimmedLine.startsWith("public ");
}

function tryParseEnumMember(trimmedLine, existingValues, nextValue) {
  if (trimmedLine.length === 0 ||
      trimmedLine.startsWith("//") ||
      trimmedLine.startsWith("[") ||
      trimmedLine.startsWith("///") ||
      trimmedLine === "{" ||
      trimmedLine === "}") {
    return null;
  }

  const match = trimmedLine.match(/^(\w+)(?:\s*=\s*([^,]+))?\s*,?$/);
  if (!match) {
    return null;
  }

  const value = resolveEnumValue(match[2], existingValues, nextValue);
  return { name: match[1], value };
}

function resolveEnumValue(expression, existingValues, nextValue) {
  if (!expression) {
    return nextValue;
  }

  const trimmedExpression = expression.trim();
  if (/^-?0x[0-9a-f]+$/i.test(trimmedExpression)) {
    return Number.parseInt(trimmedExpression, 16);
  }

  if (/^-?\d+$/.test(trimmedExpression)) {
    return Number.parseInt(trimmedExpression, 10);
  }

  if (existingValues.has(trimmedExpression)) {
    return existingValues.get(trimmedExpression);
  }

  return nextValue;
}

function splitTypeReferences(typeList) {
  return (typeList ?? "")
    .split(",")
    .map(entry => entry.trim())
    .filter(Boolean);
}

function resolveTypeReference(reference, namespaceName, lookup, publicTypes) {
  if (!reference) {
    return "";
  }

  const cleanedReference = reference
    .replace(/\s+where\s+.+$/, "")
    .replace(/<.*$/, "")
    .trim();

  if (lookup.has(cleanedReference)) {
    return lookup.get(cleanedReference);
  }

  const namespacedReference = `${namespaceName}.${cleanedReference}`;
  if (publicTypes.has(namespacedReference)) {
    return namespacedReference;
  }

  return publicTypes.has(cleanedReference)
    ? cleanedReference
    : "";
}

function renderTypeExpression(typeExpression, publicApiSurface) {
  if (!typeExpression) {
    return "";
  }

  return typeExpression
    .replace(/\s*,\s*/g, ", ")
    .replace(/</g, "<")
    .replace(/>/g, ">")
    .replace(/\b([A-Za-z_][A-Za-z0-9_.]*)\b/g, match => renderTypeToken(match, publicApiSurface));
}

function renderTypeToken(token, publicApiSurface) {
  const builtInAliases = new Set([
    "bool",
    "byte",
    "char",
    "decimal",
    "double",
    "float",
    "int",
    "long",
    "nint",
    "nuint",
    "object",
    "sbyte",
    "short",
    "string",
    "uint",
    "ulong",
    "ushort",
    "void"
  ]);
  const parameterModifiers = new Set(["in", "out", "ref", "params"]);

  if (parameterModifiers.has(token) || builtInAliases.has(token)) {
    return `\`${token}\``;
  }

  const resolvedType = resolveTypeReference(token, "", publicApiSurface.simpleTypeLookup, publicApiSurface.publicTypes);
  if (resolvedType) {
    return renderTypeLink(resolvedType);
  }

  const simplifiedToken = token
    .replace(/^System\./, "")
    .replace(/^Microsoft\.Extensions\.Logging\./, "")
    .replace(/^RoyalApps\.Community\.Rdp\.WinForms\./, "");
  return `\`${simplifiedToken}\``;
}

function collectReferencedPublicTypes(member, namespaceName, lookup, publicTypes) {
  const referencedTypes = new Set();
  const expressions = [];

  if (member.type) {
    expressions.push(member.type);
  }

  if (member.returnType) {
    expressions.push(member.returnType);
  }

  for (const parameter of member.parameters ?? []) {
    if (parameter.type) {
      expressions.push(parameter.type);
    }
  }

  for (const expression of expressions) {
    for (const token of expression.match(/\b[A-Za-z_][A-Za-z0-9_.]*\b/g) ?? []) {
      const resolvedType = resolveTypeReference(token, namespaceName, lookup, publicTypes);
      if (resolvedType) {
        referencedTypes.add(resolvedType);
      }
    }
  }

  return referencedTypes;
}

function getResolvedDocs(memberId, stack = new Set()) {
  if (resolvedMemberDocs.has(memberId)) {
    return resolvedMemberDocs.get(memberId);
  }

  const rawDocs = memberDocs.get(memberId) ?? emptyDocs();
  if (stack.has(memberId)) {
    return stripInheritdoc(rawDocs);
  }

  stack.add(memberId);
  let inheritedDocs = emptyDocs();
  const inheritdocTarget = resolveInheritdocTarget(memberId, rawDocs, stack);
  if (inheritdocTarget) {
    inheritedDocs = getResolvedDocs(inheritdocTarget, stack);
  }

  const resolvedDocs = mergeDocs(inheritedDocs, rawDocs);
  resolvedMemberDocs.set(memberId, resolvedDocs);
  stack.delete(memberId);
  return resolvedDocs;
}

function resolveInheritdocTarget(memberId, docs, stack) {
  if (!docs.hasInheritdoc) {
    return "";
  }

  if (docs.inheritdocCref && docs.inheritdocCref !== memberId) {
    return docs.inheritdocCref;
  }

  const memberKind = memberId[0];
  const declaringType = memberKind === "T" ? memberId.slice(2) : getOwningType(memberId);
  return findInheritedMember(memberId, declaringType, stack);
}

function resolveInheritedMemberCref(memberId, memberName) {
  const declaringType = getOwningType(memberId);
  const typeInfo = publicApi.typeInfos.get(declaringType);
  if (!typeInfo) {
    return "";
  }

  for (const relatedType of typeInfo.relatedTypes) {
    const exactCandidateId = `${memberId[0]}:${relatedType}.${memberName}`;
    if (memberDocs.has(exactCandidateId)) {
      return exactCandidateId;
    }

    if (memberId[0] === "M") {
      const methodCandidates = Array.from(memberDocs.keys()).filter(id =>
        id.startsWith(`M:${relatedType}.${memberName}(`));
      if (methodCandidates.length === 1) {
        return methodCandidates[0];
      }
    }
  }

  return "";
}

function findInheritedMember(memberId, typeName, stack, visitedTypes = new Set()) {
  if (!typeName || visitedTypes.has(typeName)) {
    return "";
  }

  visitedTypes.add(typeName);
  const typeInfo = publicApi.typeInfos.get(typeName);
  if (!typeInfo) {
    return "";
  }

  for (const relatedType of typeInfo.relatedTypes) {
    const candidateId = memberId[0] === "T"
      ? `T:${relatedType}`
      : `${memberId[0]}:${relatedType}.${getMemberSuffix(memberId)}`;
    if (memberDocs.has(candidateId) && !stack.has(candidateId)) {
      return candidateId;
    }

    const inheritedCandidate = findInheritedMember(memberId, relatedType, stack, visitedTypes);
    if (inheritedCandidate) {
      return inheritedCandidate;
    }
  }

  return "";
}

function mergeDocs(inheritedDocs, ownDocs) {
  return {
    summary: ownDocs.summary || inheritedDocs.summary,
    remarks: ownDocs.remarks || inheritedDocs.remarks,
    returns: ownDocs.returns || inheritedDocs.returns,
    externalLinks: ownDocs.externalLinks.length > 0 ? ownDocs.externalLinks : inheritedDocs.externalLinks,
    inheritdocCref: "",
    hasInheritdoc: false,
    params: ownDocs.params.length > 0 ? ownDocs.params : inheritedDocs.params
  };
}

function stripInheritdoc(docs) {
  return {
    summary: docs.summary,
    remarks: docs.remarks,
    returns: docs.returns,
    externalLinks: docs.externalLinks,
    inheritdocCref: "",
    hasInheritdoc: false,
    params: docs.params
  };
}

function countOccurrences(text, token) {
  return text.split(token).length - 1;
}
