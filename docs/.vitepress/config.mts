import { defineConfig } from "vitepress";
import apiSidebar from "../api/sidebar.mjs";

const guideItems = [
  { text: "Overview", link: "/" },
  { text: "Getting Started", link: "/articles/getting-started" },
  { text: "Support Matrix", link: "/articles/support-matrix" },
  { text: "External Mode", link: "/articles/external-mode" },
  { text: "Migrate from v1", link: "/articles/migrating-from-v1" }
];

export default defineConfig({
  title: "RoyalApps RDP",
  description: "WinForms Remote Desktop hosting for embedded ActiveX and external mstsc.exe sessions.",
  base: "/royalapps-community-rdp/",
  cleanUrls: true,
  themeConfig: {
    logo: "/assets/RoyalApps_1024.png",
    nav: [
      { text: "Guide", link: "/articles/getting-started" },
      { text: "API", link: "/api/" },
      { text: "GitHub", link: "https://github.com/royalapplications/royalapps-community-rdp" }
    ],
    sidebar: {
      "/articles/": [
        { text: "Guide", items: guideItems },
        { text: "API", items: apiSidebar }
      ],
      "/api/": [
        { text: "Guide", items: guideItems },
        { text: "API", items: apiSidebar }
      ]
    },
    socialLinks: [
      { icon: "github", link: "https://github.com/royalapplications/royalapps-community-rdp" }
    ],
    search: {
      provider: "local"
    },
    footer: {
      message: "MIT Licensed",
      copyright: "Copyright Royal Apps GmbH"
    }
  }
});
