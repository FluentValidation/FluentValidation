require 'rexml/document'
include REXML

module TeamCity
	def teamcity_progress(task)
		teamcity_service_message 'progressStart', task
		yield if block_given?
		teamcity_service_message 'progressFinish', task
	end
	
	def teamcity_service_message(type = '', message = '')
		puts "##teamcity[#{type} '#{message.encode}']" if ENV['TEAMCITY_PROJECT_NAME']
	end
	
	def self.add_statistic(key = '', value = '')
		puts "##teamcity[buildStatisticValue key='#{key.to_s.encode}' value='#{value.to_s.encode}']" if ENV['TEAMCITY_PROJECT_NAME']
	end
	
	def self.import_data(type = '', path = '')
		puts "##teamcity[importData type='#{type.to_s.encode}' path='#{path.to_s.encode}']" if ENV['TEAMCITY_PROJECT_NAME']
	end
	
	def self.append_build_status_text(text = '')
		return if not ENV['TEAMCITY_PROJECT_NAME']
		
		teamcity_info = 'teamcity-info.xml'

		if File.exists?(teamcity_info)
			doc = Document.new File.read(teamcity_info) 
		else
			doc = Document.new ''
		end
		
		XPath.first(doc, "//").add_element('build') if not XPath.first(doc, "//build")
		XPath.first(doc, "//build").add_element('statusInfo') if not XPath.first(doc, "//build/statusInfo")
		XPath.first(doc, "//build/statusInfo").add_element('text', {'action' => 'append'}).text = text
	
		out = ''
		doc.write out
		File.open(teamcity_info, 'w') do |f|
			f.write(out)
		end
	end
end

class String
	def encode
		self \
			.gsub(/\|/, "||") \
			.gsub(/'/, "|'") \
			.gsub(/\n/, "|n") \
			.gsub(/\r/, "|r") \
			.gsub(/\]/, "|]")
	end
end

class Rake::Task
	include TeamCity
	old_execute = self.instance_method(:execute)
	
	define_method(:execute) do |args|
		teamcity_progress("Executing #{name} rake task") do
			old_execute.bind(self).call(args)
		end
	end
end