# Cloud Foundry .NET Core S3 Demo
This sample code shows how to access an S3 Blobstore and perform certain operations.
The sample code has operations for file list, downloads, redirect and uploads.

Note that the code here is not production ready code.

# What you will need to get started

1. An S3 blobstore.  Sign up for the service at http://aws.amazon.com/s3 
2. A pre-created bucket - Replace <BLOB> in S3FileController with your bucket name.
3. The Access Key and Secret Access Key - Replace ACCESS_KEY and ACCESS_KEY_SECRET in Startup.cs.  Again this is not secure, so don't do this in production and don't check your keys into a repository!
4. Ensure you havet the latest .NET Core SDK installed, CF tools, CF account setup and logged in via cf login. Free account at Pivotal Web Services at http://run.pivotal.io/

# Running the example

1. Do the above steps
2. Run ./gocf.sh

# Some Operations

1. List:  <route>/S3File/list
2. Download: <route>/S3File/download/<file-in-bucket>
3. Redirect: <route>/S3File/redirect/<file-in-bucket>
4. Upload Files: <route>/S3File/UploadFiles
