#!/usr/bin/env ruby

proj_dir = File.dirname(__FILE__)

`xbuild /p:Configuration=Debug "#{proj_dir}/EventBuilder.csproj"`

dlls = ARGV.to_a.map {|x| "\"#{x}\"" }.join ","
puts dlls
to_execute = "mono #{proj_dir}/EventBuilder.exe #{dlls} #{proj_dir}/Events.mustache > Events.cs"
#puts to_execute
`#{to_execute}`

to_delete = ["EventBuilder.exe", "EventBuilder.exe.mdb", "Mono.Cecil.dll", "Nustache.Core.dll"].map {|x| "\"#{File.join(proj_dir, x)}\"" }
`rm #{to_delete.join " "}`
