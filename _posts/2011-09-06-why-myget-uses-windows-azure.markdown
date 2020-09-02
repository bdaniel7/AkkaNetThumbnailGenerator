---
layout: post
title: "Why MyGet uses Windows Azure"
date: 2011-09-06 13:55:00 +0000
comments: true
published: true
categories: ["post"]
tags: ["ASP.NET", "Azure", "CSharp", "General", "MVC", "Scalability", "Webfarm"]
alias: ["/post/2011/09/06/Why-MyGet-uses-Windows-Azure.aspx", "/post/2011/09/06/why-myget-uses-windows-azure.aspx"]
author: Maarten Balliauw
redirect_from:
 - /post/2011/09/06/Why-MyGet-uses-Windows-Azure.aspx.html
 - /post/2011/09/06/why-myget-uses-windows-azure.aspx.html
---
<p><a href="http://www.myget.org" target="_blank"><img style="margin: 0px 0px 5px 5px; display: inline; float: right;" title="MyGet - NuGet hosting private feed" src="http://www.myget.org/content/themes/myget/logo.png" alt="MyGet - NuGet hosting private feed" width="250" height="78" align="right" /></a>Recently one of the Tweeps following me started fooling around and hit one of my sweet spots: Windows Azure. Basically, he mocked me for using Windows Azure for <a href="http://www.myget.org" target="_blank">MyGet</a>, a website with enough users but not enough to justify the &ldquo;scalability&rdquo; aspect he thought Windows Azure was offering. Since Windows Azure is much, much more than scalability alone, I decided to do a quick writeup about the various reasons on why we use <a href="http://www.azure.com" target="_blank">Windows Azure</a> for MyGet. And those are not scalability.</p>
<p>First of all, here&rsquo;s a high-level overview of our deployment, which may illustrate some of the aspects below:</p>
<p><a href="/images/image_142.png"><img style="background-image: none; padding-left: 0px; padding-right: 0px; display: block; float: none; margin-left: auto; margin-right: auto; padding-top: 0px; border: 0px;" title="image" src="/images/image_thumb_110.png" border="0" alt="image" width="404" height="191" /></a></p>
<h2>Costs</h2>
<p>Windows Azure is <em>cheap</em>. Cheap as in cost-effective, not as in, well, sleezy. Many will disagree with me but the cost perspective of Windows Azure can be real cheap in some cases as well as very expensive in other cases. For example, if someone asks me if they should move to Windows Azure and they now have one server running 300 small sites, I&rsquo;d probably tell them not to move as it will be a tough price comparison.</p>
<p>With MyGet we run 2 Windows Azure instances in 2 datacenters across the globe (one in the US and one in the EU). For $180.00 per month this means 2 great machines at two very distant regions of the globe. You can probably find those with other hosters as well, but will they manage your machines? Patch and update them? Probably not, for that amount. In our scenario, Windows Azure is cheap.</p>
<p>Feel free to look at the <a href="http://www.microsoft.com/windowsazure/pricing-calculator/" target="_blank">cost calculator tool</a> to estimate usage costs.</p>
<h2>Traffic Manager</h2>
<p>Traffic Manager, a great (beta) product in the Windows Azure offering allows us to do geographically distributed applications. For example, US users of MyGet will end up in the US datacenter, European users will end up in the EU datacenter. This is great, and we can easily add extra locations to this policy and have, for example, a third location in Asia.</p>
<p>Next to geographically distributing MyGet, Traffic Manager also ensures that if one datacenter goes down, the DNS pool will consist of only &ldquo;live&rdquo; datacenters and thus provide datacenter fail-over. Not ideal as the web application will be served faster from a server that&rsquo;s closer to the end user, but the application will not go down.</p>
<p>One problem we have with this is storage. We use Windows Azure storage (blobs, tables and queues) as those only cost $0.12 per GB. Distributing the application does mean that our US datacenter server has to access storage in the EU datacenter which of course adds some latency. We try to reduce this using extensive caching on all sides, but it&rsquo;d be nicer if Traffic Manager allowed us to setup georeplication for storage as well. This only affects storing package metadata and packages. Reading packages is not affected by this because we&rsquo;re using the Windows Azure CDN for that.</p>
<h2>CDN</h2>
<p>The Windows Azure Content Delivery Network allows us to serve users fast. The main use case for MyGet is accessing and downloading packages. Ok, the updating has some latency due to the restrictions mentioned above, but if you download a package from MyGet it will always come from a CDN node near the end user to ensure low latency and fast access. Given the CDN is just a checkbox on the management pages means integrating with CDN is a breeze. The only thing we&rsquo;ve struggled with is finding an acceptable caching policy to ensure stale data is limited.</p>
<h2>Windows Azure AppFabric Access Control</h2>
<p>MyGet is not one application. MyGet is three applications: our development environment, staging and production. In fact, we even plan for tenants so every tenant in fact is its own application. To streamline, manage and maintain a clear overview of which user can authenticate to which application via which identity provider, we use ACS to facilitate MyGet authentication.</p>
<p>To give you an example: our dev environment allows logging in via OpenID on a development machine. Production allows for OpenID on a live environment. In staging, we only use Windows Live ID and Facebook whereas our production website uses different identity providers. Tenants will, in the future, be given the option to authenticate to their own ADFS server, we&rsquo;re pretty sure ACS will allow us to simply configure that and instrument only tenant X can use that ADFS server.</p>
<p>ACs has been a great time saver and is definitely something we want to use in future project. It really eases common authentication pains and acts as a service bus between users, identity providers and our applications.</p>
<h2>Windows Azure AppFabric Caching</h2>
<p>Currently we don&rsquo;t use Windows Azure AppFabric Caching in our application. We currently use the ASP.NET in-memory cache on all machines but do feel the need for having a distributed caching solution. While appealing, we think about deploying Memcached in our application because of the cost structure involved. But we might as well end up with Wndows Azure AppFabric Caching anyway as it integrates nicely with our current codebase.</p>
<h2>Conclusion</h2>
<p>In short, Windows Azure is much more than hosting and scalability. It&rsquo;s the building blocks available such as Traffic Manager, CDN and Access Control Service that make our lives easier. The pricing structure is not always that transparent but if you dig a little into it you&rsquo;ll find affordable solutions that are really easy to use because you don&rsquo;t have to roll your own.</p>
{% include imported_disclaimer.html %}
