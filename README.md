# AI knowledge exchange 2025-09-03

A showcase of my agentic coding setup (mostly for, but not limited to, dotnet development)








































## About me

- Jonathan (Jon)
- 10 years with Zühlke
- web dev -> .NET lead architect -> principal software architect
  - lots of different kinds of project from hands-on coding to solution architecture to cloud consulting to performance / stability task forces
- currently supporting a bank in their Azure migration




















































## Why are we here?

- agentic coding is a hot topic
  - there are 3 parallel tracks today
  - clients demand efficiency gains
- rapidly developing field
  - the tools are evolving
  - the usage patterns are evolving
  - we need to iterate just as rapdily and exchange experiences and ideas


















































## Disclaimer

I claim neither correctness nor completeness nor originality of thought














































## My Design Goals for agentic coding

- the setup is agent-agnostic as much as possible
- any agent runs in a secure sandbox, so that it can run with maximal permissions
- we manage the context as explicitly as we can
- the agent and the user can collaborate on the same code
- get the agent to create well-architected, well-tested code
- enable agent to iterate indepdendently until it succeeds
- get the agent to emit consistent code
  - code style
  - code quality

















































## My Constraints for agentic coding

- the agent runs in terminal, not in IDE
- no MCP, just a set of curated CLI commands
  - they pollute the context
  - you do not control what they do
- the user manages dependencies explicitly
- dotnet-specific: code style should be consistent regardless of IDE, i.e. minimal use of Resharper/Rider specific settings
- dotnet-specific: the agent runs in a container in WSL, but the IDE runs on Windows


















































### My influences

- Armin Ronacher
  - Python dev, creator of Flask, Jinja, and other projects
  - spends lots of time investigating these tools, and shares experiences on youtube
    - e.g. [this video](https://www.youtube.com/watch?v=Y4_YYrIKLac)



















































## The tools I use

- Claude Code
- Gemini CLI
- opencode
- podman
- Rider
- Roslyn Analyzers
  - Microsoft Analyzers
  - Roslynator
  - Meziantou
  - StyleCop
  - SonarAnalyzers
- CSharpier (tool and IDE plugin)
- Roslynator CLI






























































## Code Showcase

- agent container images
- agent config files
- `AGENTS.md`
  - vertical slice architecture
  - using libraries
  - testing strategy
  - development style
  - more control over commands with `justfile`
- `.editorconfig`
  - formatting rules
  - analyzers
- testing
- file system sync














































## A more complex example

- the WSL FS Git Sync project










































## Conclusions

- your tooling setup matters A LOT for the quality of the output
- a good `AGENTS.md` is imperative
- it is absolutely possible to have an efficient agentic coding loop with C#
- you can get an agent to follow your code style and architecture
- Claude Code worked best for me
- while async coding is possible, there are still benefits to watching the agent as it works, and interrupt it if necessary

### Challenges

- WSL and Windows file sync requires custom app
- Rider often goes out of sync since changes happen outside of it
- C# has too many options, so we need to lock in as many choices as we can using analyzers in order to get consistent output
  - some options are never learnt, forcing unnecessary loop iterations
