<h2>BookieBreaker Squad Registration Service Stack:</h2>

<h3>BookieBreaker Ecosystem Information</h3>

<p>
	The BookieBreaker software stack is broken into two categories;
</p>

<ul>
	<li>Data Extraction - series of scalable micro services that will scrape data from WhoScored.coms data feeds</li>
	<li>Data Utilisation - series of applications that make use of the acquired data via REST API's</li>
</ul>

<p>
	WhoScored.com has a wealth of information available for seasons, clubs, players, fixtures & match statistics.
	All data extraction services will communicate via an Azure Service Bus event pipeline.
	Note that all data extration services have request throttling to ensure that the endpoints don't spam the server.
</p>

<h3>BookieBreaker Squad Registration Service Information</h3>

<p>The BookieBreaker Squad Registration Service is a single stand alone micro service which is part of the larger 'BookieBreaker' micro service ecosystem.</p>

<p>The service has a sole responsibility for exracting player and player club season participation data and passing this off to the squad resgistration API to be stored in a data repo.</p>

<p>The service is triggered by the creation of new club season associations by way of a BookieBreaker Service Bus.</p>

<h3>BookieBreaker Season Participants Service Development & Deployment Notes</h3>

<p>
	Master branch is configured to trigger CI/CD for both service components (API & Svc) for build and deployment to staging environment on Azure. 
	Any development should be done in a local feature branch (sourced from develop branch) and pull requests should be raised for review and merge to develop.
	Project owner will be responsible for merging develop branch into master and management of CI/CD pipeline.
	Currently no CI/CD tasks exists for creation of the Azure Service Bus, App Service Instances or SQL database - these exist from manual setup in stage environment currently and the CI/CD is just building and deploying the codebase.
</p>

<p>The Service consists of the following components:
	<ul>
		<li>Squad Registration API - responsible for managing squad registration data interactions with the underlying data container</li>
		<li>Squad Registration Extration Svc - responsible for parsing and extracting season participant data</li>
	</ul>
</p>

<p><b>Requires Asp.Net Core 2.2</b></p>