# CSGOHoursChecker
Check hours of CSGO in steam accounts and get output in CSV formate.

### USAGE
- Add accounts into `accounts.txt` file in following formate:
 ` steamid password steamprofilelink `
 steamid and password is only there so that its easier to identify accounts in the result CSV file.
- On Windows, run CSGOHoursChecker.exe file || on Linux, install dotnet 5.0 libraries and then run CSGOHoursChecker after granting it execute permissions.
- output will be printed in CSV formate in `done.txt` file in the same folder. failed accounts (account might have private profile or suspension) will be written in `failed.txt` file.
