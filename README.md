# GearhostUtilities

Helpful utilities for working with websites and databases hosted with GearHost.com.

Current release focuses on providing a utility for regular database backup restoration.  Future releases, after supporting releases from GearHost will provide the same service for sites.  Additional features will be added upon request.

## Database Backup Job
GearHost does a great job of managing backups, however, we like to be overly cautious and keep backups locally in addition to those that are managed by the hosting provider.  This job is the default operation for the GearHostRunner executable.  

### Configuration
The following configruation elements are available in the exe.config file.

		<appSettings>
			<add key="GearHostApiKey" value="YOUR-API-KEY-HERE"/>
			<add key="GearHostApiBaseUrl" value="https://api.gearhost.com/v1/"/>
			<add key="BackupDatabaseJob-SkipDatabases" value="dbtoskip"/>
			<add key="LocalFileStorageService-RootPath" value=""/>
			<add key="LocalFileStorageService-MaxVersions" value="5"/>
		</appSettings>

Setting | Value
--- | ---
GearHostApiKey | Your API key, obtained from your my.gearhost.com account
GearHostApiBaseUrl | The base URL to the GearHost API, with trailing slash.  Shouldn't require changes
BackupDatabaseJob-SkipDatabases | A comma-separated listing of databases that SHOULD NOT be backed up.  Helpful for test databases or others you desire to not backup
LocalFileStorageService-RootPath | File path that will be used locally for storing backups.  If left blank folders will be created in the directory of the executable
LocalFileStorageService-MaxVersions | How many versions should be retained.  Regardless of frequency, this will ensure that you only have the specified number of backup versions.

### Execution
The application is all driven by configuration so it needs no parameters, and is setup in a manner that will allow consistent execution without user interaction.  Windows Task Scheduler can be used to run the job on a regular basis.  We use a once-per-day schedule for our operations, you can configure in any manner desired.

## Feature Requests
We encourage feature requests and bug reports.  Feel free to submit issues/requests as needed using the [Issues](https://github.com/IowaComputerGurus/GearhostUtilities/issues) tab above and we will work to address them as soon as possible.

		
