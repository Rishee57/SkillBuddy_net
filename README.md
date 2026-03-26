(The file `/home/parrot/RiderProjects/SkillBuddy/README.md` exists, but is empty)

## Development helper: safe dev-run

If you frequently run the app during development you can use the helper script `SkillBuddy.Blazor/scripts/dev-run.sh` which will attempt to stop any process listening on the requested port and then start `dotnet run` on that port.

Usage:

```bash
# run on default 5116
cd SkillBuddy.Blazor
./scripts/dev-run.sh

# run on a specific port (example: 5117)
./scripts/dev-run.sh 5117
```

This prevents the common case where a previous debug instance is still holding the port (executable may have been deleted after rebuilds) and causes Kestrel to fail with "Address already in use".
