require 'rake'
require 'erb'

class AssemblyInfoBuilder
	attr_reader :attributes

	def initialize(attributes)
		@attributes = attributes;
	end

	def write(file)
		template = %q{
			using System;
			using System.Reflection;
			using System.Runtime.InteropServices;

			<% @attributes.each do |key, value| %>
				[assembly: <%= key %>("<%= value %>")]
			<% end %>
		}.gsub(/^\s+/, '')

		erb = ERB.new(template, 0, "%<>")

		File.open(file, 'w') do |f|
			f.puts erb.result(binding)
		end
		
		puts "Created file #{file}"
	end
end