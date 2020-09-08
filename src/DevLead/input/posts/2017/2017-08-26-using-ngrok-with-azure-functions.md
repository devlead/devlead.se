---
title: Using ngrok with Azure Functions⚡
tags:
    - API
    - Azure
    - Azure Functions
    - Ngrok
    - Serverless
author: devlead
published: 2017-08-20
canonical: https://medium.com/hackernoon/using-ngrok-with-azure-functions-7e209e96538c
description: Serverless debugging on steroids💪
---

With things like the Azure Functions Cli and Azure Functions tools for Visual Studio you get the full development and debugging story locally on your machine. This is great as you can iterate and test quickly without the need to push the code to the cloud first, the drawback of this is that you can’t do incoming webhooks from. 3:rd party services, i.e. GitHub can’t access your locally running function.

But what if I said there’s a way you have your cake and eat it, wouldn’t that be great?

## ngrok

Introducing ngrok, ngrok is a tool and a service that will let you securely inspect and tunnel traffic to your local computer. It’s a free service with paid plans that will give you extra features like custom and reserved domains, IP address whitelisting etc.

## Obtaining the tool

ngrok is available cross platform for MacOS, Windows, Linux and FreeBSD and it’s just a single binary you can download and unzip from [ngrok.com/download](https://ngrok.com/download). If you’re running Chocolatey on Windows like me, then it’s just a simple command way to get it installed:
`choco install -y ngrok.portable`

## Using ngrok

Using ngrok is very straightforward, in general you launch the tool with which protocol and port your local service is listening on.
`ngrok http 8080`

![ngrok running](https://cdn.devlead.se/clipimg-vscode/2021/01/11/75ba557b-50d3-94cc-39a6-e330a127244c.png?sv=2019-12-12&st=2021-01-10T13%3A04%3A11Z&se=2031-01-11T13%3A04%3A11Z&sr=b&sp=r&sig=MJNZ3xMphCitS5%2FwHM3yUcBrHAcf2W%2BG8SWt1xLjSz8%3D)

ngrok will launch and the forwarding urls is what you use to access your local service from the Internet.

## Using ngrok with Azure Functions

You can find out which local port your Azure Functions by looking at the output of when the host starts, you can also specify the port using the port switch when launching the functions host
`func host -p 8080`

![Azure Functions Host port](https://cdn.devlead.se/clipimg-vscode/2021/01/11/3cc00ea9-6f20-c435-c5eb-85ed3c08804d.png?sv=2019-12-12&st=2021-01-10T13%3A05%3A19Z&se=2031-01-11T13%3A05%3A19Z&sr=b&sp=r&sig=sbfyFtGZwwBnqMDpi%2FiU4QyJ9oweCeb5cU6URfbaYG8%3D)

By default, ngrok will forward it’s temporary domain as host header to the locally running service, but as The Azure Functions host only listens to the hostname “localhost” we’ll need to override the default behavior using the host header switch
`ngrok http 8080 --host-header localhost`

## Externally calling local function

To use your locally running function externally you just replace the base url provided by the function host to the temporary url provided by ngrok i.e.
`http://localhost:8080` becomes `https://tempsubdomain.ngrok.io`.

So say you have a GitHub webhook called `GithubWebhookCSharp` its local url will be `http://localhost:8080/api/GithubWebhookCSharp` and it’s external url will be `https://tempsubdomain.ngrok.io/api/GithubWebhookCSharp`.

Which you then could set up as i.e. a GitHub webhook

![Setting up webhook on GitHub](https://cdn.devlead.se/clipimg-vscode/2021/01/11/fd80337f-d020-ae4f-e2fd-b60f60843b74.png?sv=2019-12-12&st=2021-01-10T13%3A06%3A49Z&se=2031-01-11T13%3A06%3A49Z&sr=b&sp=r&sig=nRjbhYTdQuW0%2BGbz8RvZUbX6PI0OdLsyUaLw%2BDBWYfo%3D)

now when GitHub webhook triggers it’ll tunnel through ngrok and its payload will be delivered to your locally otherwise externally inaccessible function

![ngrok / Azure function host serving local function](https://cdn.devlead.se/clipimg-vscode/2021/01/11/03bb2043-2dcf-d789-ffa3-fe26079537b9.png?sv=2019-12-12&st=2021-01-10T13%3A09%3A19Z&se=2031-01-11T13%3A09%3A19Z&sr=b&sp=r&sig=aAtxSvy0er9zBq5q0s3Vl6JXzxUn3s7lh0PQGtkl0tw%3D)

## Inspecting traffic

On off the real killer features of ngrok is that it provides a local web interface, you’ll find it’s url in the tool’s output

![ngrok web interface url](https://cdn.devlead.se/clipimg-vscode/2021/01/11/c4c98d7a-a7c6-e009-ebe1-0e2e0d85f036.png?sv=2019-12-12&st=2021-01-10T13%3A09%3A48Z&se=2031-01-11T13%3A09%3A48Z&sr=b&sp=r&sig=0emxBg1Jsllf974MaJ7dVZXrYM3ll2qyhPS%2BOoAfJUY%3D)

This interface provides deep insight of all traffic that travels through the ngrok tunnel, you can see the response/request body and headers, it also lets you replay a request as many times as you like without needing to trigger an event on the external service, which is really useful when debugging, iterating over an implementation and fixing bugs!

![ngrok web interface](https://cdn.devlead.se/clipimg-vscode/2021/01/11/a5d64ad8-64c9-b05c-37cd-ab487c32e480.png?sv=2019-12-12&st=2021-01-10T13%3A10%3A11Z&se=2031-01-11T13%3A10%3A11Z&sr=b&sp=r&sig=3n1GdorjYDsMbGzCcsGY3ng%2B7lmLZk2IBMdhk9KC9WY%3D)

## Conclusion

Using a service like ngrok is really powerful and can ultimately speed up the development process.
