require 'rake'
require 'erb'

class QuickTemplate
	attr_reader :args, :file
	
	def initialize(file)
		raise "The template file to process must be given" if file.nil?

		@file = file
	end
	
	def exec(args = {})
		template = prepare_template File.read(@file)
		result = exec_erb template, args
		
		resultFile = @file.ext('')
		File.open(resultFile, 'w') do
			|f| f.write(result)
		end
		
		puts "Created file #{resultFile}"
	end
	
	def prepare_template(template)
		tag_regex = /(?:\W)(@\w[\w\.]+\w@)(?:\W)/
		
		hits = template.scan(tag_regex)
		tags = hits.map do |item|
			item[0].chomp('@').reverse.chomp('@').reverse.strip
		end
		
		tags.map! do |a|
			a.to_sym
		end
		tags.uniq!

		tags.inject(template) do |text, tag|
			text.gsub /@#{tag.to_s}@/, "<%= #{tag.to_s} %>"
		end
	end
		
	def exec_erb(template, args)
		b = binding
		erb = ERB.new(template, 0, "%")
		erb.result(b)
	end
end