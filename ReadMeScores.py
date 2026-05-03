import json
import re
import tkinter as tk
from tkinter import filedialog, messagebox

MAX_ENTRIES = 10

def parse_readme_scores(content):
    match = re.search(r"<!-- LEADERBOARD_START -->(.*?)<!-- LEADERBOARD_END -->", content, re.DOTALL)
    if not match:
        return []
    entries = []
    for line in match.group(1).strip().split("\n"):
        m = re.match(r"\d+\.\s+(\w+)\s+(\d+) pts\s+([\d:]+)\s+(\d+) kills", line.strip())
        if m:
            entries.append({
                "playerName": m.group(1),
                "score": int(m.group(2)),
                "time": m.group(3),
                "zombiesKilled": int(m.group(4)),
                "rawTime": sum(int(x) * 60**i for i, x in enumerate(reversed(m.group(3).split(":"))))
            })
    return entries

def format_leaderboard(entries):
    lines = []
    for i, e in enumerate(entries):
        lines.append(f"{i+1}. {e['playerName']}  {e['score']} pts  {e['time']}  {e['zombiesKilled']} kills")
    return "\n".join(lines)

def get_local_scores_from_file(path):
    try:
        import winreg
        # path is a registry key path like Software\CompanyName\GameName
        key = winreg.OpenKey(winreg.HKEY_CURRENT_USER, path)
        value, _ = winreg.QueryValueEx(key, "Leaderboard")
        data = json.loads(value)
        return data.get("entries", [])
    except Exception as e:
        messagebox.showerror("Error", f"Could not read PlayerPrefs from registry:\n{e}")
        return []

def merge_scores(local, readme):
    all_entries = local + readme
    seen = set()
    unique = []
    for e in all_entries:
        key = (e["playerName"], e["score"], e["time"])
        if key not in seen:
            seen.add(key)
            unique.append(e)
    unique.sort(key=lambda x: x["score"], reverse=True)
    return unique[:MAX_ENTRIES]

def update_readme(path, content, entries):
    new_board = f"<!-- LEADERBOARD_START -->\n{format_leaderboard(entries)}\n<!-- LEADERBOARD_END -->"
    if "<!-- LEADERBOARD_START -->" in content:
        content = re.sub(r"<!-- LEADERBOARD_START -->.*?<!-- LEADERBOARD_END -->", new_board, content, flags=re.DOTALL)
    else:
        content += f"\n\n## Leaderboard\n\n{new_board}\n"
    with open(path, "w") as f:
        f.write(content)

def run():
    root = tk.Tk()
    root.title("Leaderboard Merger")
    root.geometry("500x400")
    root.resizable(False, False)

    readme_path = tk.StringVar(value="No file selected")
    registry_path = tk.StringVar(value=r"Software\DefaultCompany\COMP2007-CW2")

    def browse_readme():
        path = filedialog.askopenfilename(
            title="Select README.md",
            filetypes=[("Markdown files", "*.md"), ("All files", "*.*")]
        )
        if path:
            readme_path.set(path)

    def do_merge():
        rpath = readme_path.get()
        if rpath == "No file selected" or not rpath.endswith(".md"):
            messagebox.showerror("Error", "Please select a valid README.md file")
            return

        reg_path = registry_path.get().strip()
        if not reg_path:
            messagebox.showerror("Error", "Please enter your registry path")
            return

        try:
            with open(rpath, "r") as f:
                content = f.read()
        except Exception as e:
            messagebox.showerror("Error", f"Could not read README:\n{e}")
            return

        readme_scores = parse_readme_scores(content)
        local_scores = get_local_scores_from_file(reg_path)

        if not local_scores and not readme_scores:
            messagebox.showwarning("Warning", "No scores found in either source")
            return

        merged = merge_scores(local_scores, readme_scores)
        update_readme(rpath, content, merged)

        result = "\n".join([
            f"{i+1}. {e['playerName']}  {e['score']} pts  {e['time']}  {e['zombiesKilled']} kills"
            for i, e in enumerate(merged)
        ])
        output_text.config(state="normal")
        output_text.delete("1.0", tk.END)
        output_text.insert(tk.END, f"Merged {len(merged)} scores:\n\n{result}")
        output_text.config(state="disabled")
        messagebox.showinfo("Done", "README updated! Commit and push to share scores.")

    # UI
    tk.Label(root, text="Leaderboard Merger", font=("Arial", 14, "bold")).pack(pady=10)

    frame1 = tk.Frame(root)
    frame1.pack(fill="x", padx=20, pady=5)
    tk.Label(frame1, text="README.md:", width=14, anchor="w").pack(side="left")
    tk.Label(frame1, textvariable=readme_path, anchor="w", fg="gray").pack(side="left", fill="x", expand=True)
    tk.Button(frame1, text="Browse", command=browse_readme).pack(side="right")

    frame2 = tk.Frame(root)
    frame2.pack(fill="x", padx=20, pady=5)
    tk.Label(frame2, text="Registry Path:", width=14, anchor="w").pack(side="left")
    tk.Entry(frame2, textvariable=registry_path, width=40).pack(side="left", fill="x", expand=True)

    tk.Label(root, text="Registry path: Software\\CompanyName\\GameName\n(Edit > Project Settings > Player in Unity)",
             fg="gray", font=("Arial", 8)).pack()

    tk.Button(root, text="Merge Scores", command=do_merge,
              bg="#4CAF50", fg="white", font=("Arial", 11), padx=10, pady=5).pack(pady=10)

    output_text = tk.Text(root, height=8, state="disabled", bg="#f0f0f0")
    output_text.pack(fill="both", padx=20, pady=5)

    root.mainloop()

if __name__ == "__main__":
    run()