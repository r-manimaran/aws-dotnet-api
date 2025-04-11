This is a [Next.js](https://nextjs.org) project bootstrapped with [`create-next-app`](https://nextjs.org/docs/app/api-reference/cli/create-next-app).

## Getting Started

First, run the development server:

```bash
npm run dev
# or
yarn dev
# or
pnpm dev
# or
bun dev
```

Open [http://localhost:3000](http://localhost:3000) with your browser to see the result.

You can start editing the page by modifying `app/page.tsx`. The page auto-updates as you edit the file.

This project uses [`next/font`](https://nextjs.org/docs/app/building-your-application/optimizing/fonts) to automatically optimize and load [Geist](https://vercel.com/font), a new font family for Vercel.

## Learn More

To learn more about Next.js, take a look at the following resources:

- [Next.js Documentation](https://nextjs.org/docs) - learn about Next.js features and API.
- [Learn Next.js](https://nextjs.org/learn) - an interactive Next.js tutorial.

You can check out [the Next.js GitHub repository](https://github.com/vercel/next.js) - your feedback and contributions are welcome!

## Deploy on Vercel

The easiest way to deploy your Next.js app is to use the [Vercel Platform](https://vercel.com/new?utm_medium=default-template&filter=next.js&utm_source=create-next-app&utm_campaign=create-next-app-readme) from the creators of Next.js.

Check out our [Next.js deployment documentation](https://nextjs.org/docs/app/building-your-application/deploying) for more details.


# Deploy Next JS App to AWS S3 using GitHub CI/CD

## Steps:
1. Create the NextJs Application using the command
```bash
npx create-next-app@latest web-app
```
2. Create the S3 bucket
3. Create a IAM user to be mapped and used in Git Hub secret.
4. Download and save the Access Key and Secret Access Key securely.
5. Grant Permission to S3 bucket
6. In the Github settings create the secret for the below
    
![alt text](image-2.png)

Workflow for CI/CD Automation in .github/workflows/nextjs.yaml


```yaml
name: Deploy NextJs to AWS S3

on:
    push:
      branches:
        [ master ]
      paths:
        - 'web-app/**'
    
    workflow_dispatch:

jobs:
    deploy:
        runs-on: ubuntu-latest

        steps:
          # Step1: Checkout the repository code
          - uses: actions/checkout@v4
            with:
                submodules: true
                lfs: false
         # step 2: Setup Node.js environment
          - name: Setup Nodejs version
            uses: actions/setup-node@v4
            with:
                node-version: "22.x"
                cache: 'npm'
                cache-dependency-path: 'web-app/package-lock.json'
        # step 3: Install dependencies
          - name: Install dependencies
            working-directory: ./web-app
            run: npm install
        # step 4: Build Next.js app
          - name: Build Next.js app
            working-directory: ./web-app
            run: npm run build
        # step 5: Configure AWS Credentials
          - name: Configure AWS Credentials
            uses: aws-actions/configure-aws-credentials@v1
            with: 
                aws-access-key-id: ${{secrets.AWS_ACCESS_KEY}}
                aws-secret-access-key: ${{secrets.AWS_SECRET_ACCESS_KEY}}
                aws-region: ${{secrets.AWS_REGION}}
        # step 6: Deploy to AWS S3
          - name: Deploy to AWS S3
            working-directory: ./web-app
            run: aws s3 sync out s3://${{secrets.AWS_BUCKET}} --delete
     
```

- Perform the commit and see the auto start of the workflow.

![alt text](image-1.png)

# AWS S3 Bucket changes.

- Ensure public access is enabled in the S3 bucket.

- Add the below bucket policy. Change the bucket name in the below policy.

```json
{
    "Version": "2012-10-31",
    "Statement": [
        {
            "Sid": "PublicReadGetObject",
            "Effect": "Allow",
            "Principal": "*",
            "Action": "s3:GetObject",
            "Resource": "arn:aws:s3:::your-bucket-name/*"
        }
    ]
}
```

- Launch the application and see the next js app running from the S3 bucket.

![alt text](image.png)


npm install next-auth@beta

npm install jwt-decode

npm install react-hook-form zod @hookform/resolvers