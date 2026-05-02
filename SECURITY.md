# Security

Gremlins is a local Windows tray app that uses Win32 hooks, clipboard access, and optional startup registration. It is intended for **consenting users on their own machines** only.

## Reporting a vulnerability

If you discover a security issue in this repository (for example, unsafe deserialization, remote code execution, or credential handling), please report it responsibly:

1. **Do not** open a public GitHub issue for undisclosed vulnerabilities.
2. Contact the maintainer via a **private** channel (e.g. GitHub Security Advisories for the repo, or email if listed in the repo profile).
3. Include enough detail to reproduce: version, OS build, steps, and impact.

We will aim to acknowledge within a few days and coordinate a fix and release timeline.

## Scope notes

- The app is **not** designed as attack surface for untrusted input from the network; treat any future networking features as high-risk.
- Installer and single-file publish flows should be verified against your own threat model before enterprise deployment.
