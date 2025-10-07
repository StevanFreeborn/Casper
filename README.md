# Casper

⚠️ under construction ⚠️

This is my attempt at creating a small application that acts as my own personal ghost writer for [blog](https://blog.stevanfreeborn.com) content. It also serves as a great excuse to learn about and use the new [agent-framework](https://github.com/microsoft/agent-framework) from Microsoft.

## Workflow

```mermaid
graph TD
  UserInput([User Input: Blog Topic]) --> Researcher[Researcher Agent]
  Researcher --> Writer[Writer Agent]
  Writer --> Editor[Editor Agent]
  Editor --> Critic[Critic Agent]

  %% Critic feedback loop
  Critic -- "If REVISE needed" --> Writer
  Critic -- "If APPROVED" --> UserOutput([Final Output: Approved Blog Post])
```
