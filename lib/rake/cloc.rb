require 'rexml/document'
include REXML

class Cloc
	def self.count_loc(attributes)
		tool = attributes.fetch(:tool)
		reportFile = attributes.fetch(:report_file).to_absolute
		searchDirectory = attributes.fetch(:search_dir).to_absolute
		statistics = attributes.fetch(:statistics, {})
		
		cloc = tool.to_absolute
		
		sh "#{cloc.escape} --not-match-f=\\.Designer\\.cs$ --exclude-dir=.svn,obj,bin --xml --report-file=#{reportFile.escape} #{searchDirectory.escape}"
		
		doc = Document.new(File.read(reportFile))

		statistics.each do |key, query|
			value = XPath.first(doc, query)
			
			yield key, value if block_given?
			
			statistics[key] = value
		end
		
		return statistics
	end
end
