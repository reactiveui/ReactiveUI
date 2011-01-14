#!/usr/bin/env ruby

def main(args)
  puts Dir.glob("/Developer/MonoTouch/usr/lib/mono/*")
  versions = Dir.glob("/Developer/MonoTouch/usr/lib/mono/*").map{|x| x.gsub(/.*\//, '').to_f}
  path_to_monotouch = "/Developer/MonoTouch/usr/lib/mono/#{versions.max}/monotouch.dll"

  class_names = `monop -r:"#{path_to_monotouch}"`.lines

  class_names.each {|x| p x}
  class_names.select{|x| x =~ /Delegate$/}.each do |class_name|
    puts "Generating for #{class_name.chomp}..."
    `ruby ./generate_delegate_ios.rb "#{path_to_monotouch}" #{class_name.chomp}`
  end
end

main(ARGV)
