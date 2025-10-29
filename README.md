# Casper

⚠️ under construction ⚠️

This is my attempt at creating a small application that acts as my own personal ghost writer for [blog](https://blog.stevanfreeborn.com) content. It also serves as a great excuse to learn about and use the new [agent-framework](https://github.com/microsoft/agent-framework) from Microsoft.

## Workflow

```mermaid
flowchart TD
  user["user (Start)"];
  Interviewer["Interviewer"];
  Researcher["Researcher"];
  Writer["Writer"];
  Editor["Editor"];
  Critic["Critic"];

  user --> Interviewer;
  Interviewer -. Needs more info .-> user;
  Interviewer -. Has enough info .-> Researcher;
  Researcher --> Writer;
  Writer --> Editor;
  Editor -. Has feedback .-> Writer;
  Editor -. Has no feedback .-> Critic;
  Critic -. Has feedback .-> Writer;
```
