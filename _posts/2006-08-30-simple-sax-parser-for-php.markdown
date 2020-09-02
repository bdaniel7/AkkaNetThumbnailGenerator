---
layout: post
title: "Simple SAX parser for PHP"
date: 2006-08-30 13:16:00 +0000
comments: true
published: true
categories: ["post"]
tags: ["General", "Projects", "PHP"]
alias: ["/post/2006/08/30/simple-sax-parser-for-php.aspx"]
author: Maarten Balliauw
redirect_from:
 - /post/2006/08/30/simple-sax-parser-for-php.aspx.html
 - /post/2006/08/30/simple-sax-parser-for-php.aspx.html
---
<p>Yesterday, I was working on PRAjax. The UpdatePanel did not work completely as I wanted it to work: in the background, the whole page was still fetched and updated. A cleaner way would be to just fetch updated content and not the whole page. </p><p>In my search for a PHP HTML parsing class, I found a lot of libraries, but all with disadvantages: one was too big in file size, another only parsed XHTML, ... Luckily, I stumbled on <a href="http://www.phpclasses.org/browse/package/2140.html" mce_href="http://www.phpclasses.org/browse/package/2140.html">SAX parser</a>! So if you ever want to parse HTML and read out specific tags and attributes, try this one.</p>
{% include imported_disclaimer.html %}
