# NuGet trusted publishing

The RDP Package workflow builds and tests both target frameworks, then publishes
the resulting package on pushes to main or v1. Pull requests only build, test,
and upload an artifact.

## NuGet.org policy

Sign in to [NuGet.org](https://www.nuget.org/), open your profile menu, select
**Trusted Publishing**, and add a GitHub Actions policy with these settings:

| Field | Value |
| --- | --- |
| Package owner | The NuGet user or organization that owns RoyalApps.Community.Rdp.WinForms |
| Repository owner | royalapplications |
| Repository | royalapps-community-rdp |
| Workflow file | publish.yml |
| Environment | Leave empty |
| Package scope / glob pattern | RoyalApps.Community.Rdp.WinForms |
| Allowed operation | Publish new versions of existing packages |

Enter only the workflow filename, without the .github/workflows/ prefix.
When selecting an organization as the package owner, create the policy using an
active member account.

## GitHub configuration

In the repository's **Settings > Secrets and variables > Actions > Variables**, configure a
repository variable named **NUGET_USER** with value **StefanKoell**. This is the NuGet.org profile username
of the account that created the policy, including when the policy belongs to an
organization. The workflow reads vars.NUGET_USER, matching RoyalTerminal. A secret with the same name is a separate setting and is not used.

No GitHub environment is configured by this workflow, so leave the policy's
environment field empty.

## First publish

1. Create the NuGet policy and the NUGET_USER variable before pushing the workflow
   change to main or v1.
2. Push the change. The publish job runs only after the build and tests succeed.
   NuGet/login exchanges the GitHub identity token for a temporary publishing
   key immediately before pushing the downloaded package.
3. Check that the NuGet login and Publish package steps succeed. Existing package
   versions are skipped; the project must have a new version to upload a release.
4. After successful verification, remove the old NUGET_API_KEY GitHub secret and
   revoke the corresponding long-lived API key on NuGet.org if no other workflow
   uses it.

The policy identifies this repository and workflow, not a particular branch.
The workflow condition limits automatic publishing to pushes to main and v1.

See the [official NuGet trusted publishing documentation](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing).
