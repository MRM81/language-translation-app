
# SECURITY.md

## Purpose

This document defines the security standards, practices, and operational expectations for this project.

Security is treated as a first-class architectural concern.

---

# Security Goals

The system should:
- protect sensitive data
- minimize attack surface
- enforce least privilege
- prevent accidental exposure
- support auditing and traceability
- remain maintainable and observable

---

# Core Security Standards

## Secure Defaults

All new features should default to:
- deny-by-default behavior
- explicit authorization
- validated input
- minimal permissions
- safe failure behavior

---

## Authentication

Requirements:
- centralized authentication
- secure session/token handling
- session expiration
- secure password handling if applicable

Never:
- store plaintext passwords
- expose tokens in logs
- expose secrets to the frontend
