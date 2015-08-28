# REST'ish Chat API

Test with

## Signup

		$.ajax({ url: "/chat/signup", type: "POST", data: { username: "andreas", password: "test" } ).then(function(res) {console.log(res); });

## Post message

		$.ajax({ url: "/chat/messages", type: "POST", data: { message: "Hello" }, headers: { "X-Auth-Token": "andreas" } }).then(function(res) {console.log(res); });

## Get messages

		$.ajax({ url: "/chat/messages", type: "GET", headers: { "X-Auth-Token": "andreas" } }).then(function(res) {console.log(res); });