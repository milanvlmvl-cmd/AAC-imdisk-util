# AI Multi-Agent Orchestration Blueprint: AAC Raid Smoother

This document outlines the specialized AI Agent roles, distinct scopes, system contexts, and boundary definitions required to build, compile, and maintain the AAC Raid Smoother utility.

## 1. Agent Architecture Matrix

| Agent Role | Primary Focus Area | Technical Stack Constraints |
| :--- | :--- | :--- |
| **Lead Architect & UI Agent** | Core Form layout, life cycle, and state machine | .NET Framework 4.8, WinForms |
| **Security & Manifest Agent** | OS escalation policies & XML payload injection | UAC Subsystem, application.manifest |
| **I/O Virtualization Agent** | Disk mounting, syncing protocols, and link creation | ImDisk, Robocopy Bitmasks, Cmd redirection |
| **Process Automation Agent** | Background polling, launcher hooking, and toggle states | System.Diagnostics.Process, Async Timers |
| **Deployment Lifecycle Agent** | Resource management and non-blocking recovery | Embedded Assembly Resources, Win32 Shutdown Hooks |

---

## 2. Detailed Agent Roles & Workflows

### 🛡️ Agent 1: Lead Architect & UI Agent
* **Objective:** Establish the minimalist UI baseline and internal state coordination patterns.
* [cite_start]**System Context:** Focuses entirely on native, zero-dependency .NET 4.8 framework rules to keep compilation size under 1 MB[cite: 12, 13].
* **Tasks:**
    * [cite_start]Generate `Form1.cs` and `Form1.Designer.cs` using classic Visual Studio designer initialization standards[cite: 117, 123].
    * [cite_start]Manage central thread status mapping via `lblStatus` and coordinate execution blockades[cite: 34, 46].
* **Boundaries:** Must not implement direct kernel execution commands or file transport routines; relies on placeholders for virtualization logic.

### 🔑 Agent 2: Security & Manifest Agent
* **Objective:** Guarantee the application bypasses standard user-mode limits to manipulate system storage paths.
* [cite_start]**System Context:** Operates purely on the Windows User Account Control (UAC) boundary[cite: 14, 15].
* **Tasks:**
    * [cite_start]Construct the standard structural `app.manifest` configuring execution constraints strictly to `requireAdministrator`[cite: 27, 130].
    * [cite_start]Inject contextual developer documentation justifying the requirement for administrative escalation to the user[cite: 131, 132].
* **Boundaries:** Restricted exclusively to assembly configuration metadata and XML verification.

### 💽 Agent 3: I/O Virtualization Agent
* **Objective:** Abstract low-latency storage manipulation safely into background command loops.
* [cite_start]**System Context:** Orchestration of ImDisk Virtual Drivers, Robocopy sub-engines, and NT File System (NTFS) symbolic link hooks[cite: 7, 9, 23].
* **Tasks:**
    * [cite_start]Build the invisible command executor using explicit `ProcessStartInfo` flags[cite: 61, 62].
    * [cite_start]Handle string parsing routines to protect configuration directory paths with spaces from shell breaking[cite: 20, 21].
    * [cite_start]Evaluate Robocopy binary bitmasks specifically to trigger targeted fallbacks if codes match or exceed `8`[cite: 43, 77].
* **Boundaries:** Must not control background process pooling loops outside of explicit UI calls or handle lifecycle hooks.

### 🕹️ Agent 4: Process Automation Agent
* **Objective:** Deliver on-demand background automation by synchronizing the tool's behavior with external software engines.
* **System Context:** Process tree monitoring and asynchronous UI state evaluation.
* **Tasks:**
    * Implement an asynchronous or timer-based listener targeting `ArcheAge Classic Launcher.exe`.
    * Incorporate an active on-demand toggle via `chkAutoOptimize` to instantly connect/disconnect the monitoring thread loop.
    * Manage edge states where the launcher is restarted repeatedly without forcing redundant optimization routines.
* **Boundaries:** Must invoke the standard optimization workflows built by Agent 3 without modifying the core mounting or linking mechanisms.

### 🔌 Agent 5: Deployment Lifecycle Agent
* **Objective:** Ensure complete system portability and prevent volatile data corruption.
* [cite_start]**System Context:** High-priority operating system hooks, assembly resource manipulation, and urgent system shutdown interception[cite: 83, 93, 102].
* **Tasks:**
    * [cite_start]Write extraction logic to unpack embedded driver structures (`.exe`, `.sys`, `.inf`) dynamically to local application targets[cite: 101, 102].
    * [cite_start]Automate silent driver provisioning fallback queues utilizing Windows `SetupAPI` and `sc.exe`[cite: 110, 111].
    * [cite_start]Override WinForms `OnFormClosing` loops to parse `CloseReason` signals[cite: 83, 84]. [cite_start]Bypasses UI messaging and forces fast synchronous write-backs when encountering `WindowsShutDown` to fit within strict OS execution windows[cite: 95, 96].
* **Boundaries:** Does not alter regular operational layouts; acts purely as the application's entry setup and exit recovery wrapper.
