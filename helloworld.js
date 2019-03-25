var express = require('express');
var app = express();
app.get('/helloworld', function(req, res){
	res.writeHead(200, {'Content-Type': 'text/plain'});
	res.write('hello world!');
	return res.end();
})
app.listen(8081);
console.log('server start');
