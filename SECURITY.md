# Security

<div align="center">

[README](README.md) · [Contributing](CONTRIBUTING.md) · [Changelog](CHANGELOG.md)

</div>

---

Gremlins is a local Windows tray application. It uses Win32 hooks, clipboard access, and optional startup registration. It is intended for **consenting users on their own machines only**.

---

## Contents

- [Reporting a vulnerability](#reporting-a-vulnerability)
- [What we need from you](#what-we-need-from-you)
- [Our response](#our-response)
- [Scope notes](#scope-notes)

---

## Reporting a vulnerability

If you find a security issue in this repository (for example unsafe deserialization, remote code execution, or mishandled credentials), please report it responsibly.

| Do | Don’t |
| :-- | :-- |
| Use a **private** channel — e.g. GitHub Security Advisories for this repo, or maintainer email if listed on the profile | Open a **public** GitHub issue for an undisclosed vulnerability |

### What we need from you

Include enough detail to reproduce and assess impact:

- **Version** of Gremlins (or commit)
- **OS** build
- **Steps** to reproduce
- **Impact** (what an attacker could achieve, in your assessment)

### Our response

We aim to **acknowledge** reports within a few days and to **coordinate** a fix and release timeline with you when appropriate.

---

## Scope notes

| Topic | Note |
| :-- | :-- |
| **Network** | The app is not designed as attack surface for untrusted network input. Treat any future networking features as high-risk. |
| **Deployment** | Verify installer and single-file publish flows against **your** threat model before enterprise rollout. |
