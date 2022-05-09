# SsmsLite

Ssms Lite is a productivity extension for SQL Server Management Studio 18 based on SSMS Plus and CSV Paste.

It extends SSMS with a handful set of features:

    Query Execution History
    Schema Object Search
    Document Export for binary columns
    Pasting data copied from the Results window as formatted comma-separated values into another query’s IN clause.


# Prerequisites

SQL Server Management Studio 18

Vsix install does not work, see

https://docs.microsoft.com/en-us/sql/ssms/install-extensions-in-sql-server-management-studio-ssms?view=sql-server-ver15

# Publish workaround
Zip the extension folder in `C:\Program Files (x86)\Microsoft SQL Server Management Studio 18\Common7\IDE\Extensions\SSMSLite` as SFX package
