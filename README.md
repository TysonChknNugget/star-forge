# Star Forge

A free idle clicker game. Click the star, gather stardust, buy upgrades.

**Play / download:** https://tysonchknnugget.github.io/star-forge/

## Project layout

```
site/                     ← the website (published to GitHub Pages)
├── index.html            ← download page
├── game/star-forge.html  ← the whole game (HTML + CSS + JS)
└── downloads/            ← StarForge.exe and StarForge.zip
desktop/
├── Launcher.cs           ← Windows .exe launcher (embeds the game)
└── build.ps1             ← rebuilds the .exe and .zip
```

## Updating the game

1. Edit `site/game/star-forge.html`.
2. Rebuild the downloads:
   ```powershell
   powershell -ExecutionPolicy Bypass -File desktop\build.ps1
   ```
3. Commit and push. GitHub Actions republishes the site automatically.
