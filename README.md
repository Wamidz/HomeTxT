# HomeTxT 🏠📝

**HomeTxT** is a custom WPF text editor built with C# and .NET 10.0 in Visual Studio 2026. Purpose-built from the ground up to be aggressively over-engineered, HomeTxT prioritizes feature clutter, absurdly thorough backend routines, and experimental concepts over traditional "good coding practices."

---

## 🛠️ Project Overview & Architecture

Core features built into HomeTxT:

* **Custom WPF Interface:** Clean layout featuring dual scroll-bar navigation and streamlined workspace design.
* **File Operations:** Native file import, opening, saving, and `Save/Copy As` logic.
* **Over-Engineered Text Scrubbing (`CensorManager`):** An active content filtering engine that processes text during saves using dynamic dictionaries (`kindwords.json` and `Languages/` folder).
* **Deep Text Normalization:** Custom Leetspeak conversion and fuzzy Levenshtein distance matching algorithms designed to catch slight typos and symbol variations.
* **Aggressive Brand Protection:** Automatic pattern recognition that forcefully rewrites references to rival text editors (like Vim, VS Code, or Notepad) straight to *HomeTxT*.
* **Dictionary Whitelisting:** Integrated MIT safe-words filter to prevent valid vocabulary from triggering false positives.

---

## 🛣️ Development Roadmap (Unmapped)

HomeTxT is an evolving playground. Planned features for future releases include:

* [ ] **AI Integration:** Local/cloud LLM assistance built directly into the editing workspace.
* [ ] **In-Editor Chat:** Embedded communication channels within the text editing window.
* [ ] **Type Race Mode:** Competitive typing speed testing against other users.
* [ ] **Multiplayer Collaboration:** Real-time multi-user document editing.
* [ ] **Public Anarchy Mode:** Open-access document streams where every connected user can edit the same file simultaneously.
* [ ] **Experimental Utilities:** Additional over-designed features as development continues.

---

## ⚙️ Tech Stack & Requirements

* **Framework:** WPF (.NET 10.0)
* **IDE:** Visual Studio 2026
* **Language:** C#
* **Dependencies:** `Languages/` folder, `kindwords.json`, and `mit_words.json` (required in root directory)

---

## 📄 License

Distributed under the [MIT License](LICENSE). Open-source for modding, breaking, and over-engineering.
