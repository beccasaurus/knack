#! /usr/bin/env ruby
require 'webrick'
server = WEBrick::HTTPServer.new :Port => 3000, :DocumentRoot => Dir.pwd
trap('INT') { server.shutdown }
server.start
