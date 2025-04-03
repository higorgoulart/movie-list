# Startup

Create a file `.env` file on `src`, following this example:

```
TMDB_ACCESS_TOKEN=""
```

Execute `src\start.bat` (maybe you will need to change the MSBuild path).

Now, all you need is running.

- Worker: [http://localhost:5000/hangfire](http://localhost:5000/hangfire);
- API: [http://localhost:5001/swagger](http://localhost:5001/swagger);
- Frontend: [http://localhost:5173](http://localhost:5173).
