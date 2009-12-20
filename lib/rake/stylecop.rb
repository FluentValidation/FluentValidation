require 'rexml/document'
include REXML

class StyleCop
	def self.analyze(attributes)
		tool = attributes.fetch(:tool)
		directories = attributes.fetch(:directories, [])
		recurse = attributes.fetch(:recurse, true)
		files = attributes.fetch(:files, [])
		ignoreFilePattern = attributes.fetch(:ignore_file_pattern, [])
		settingsFile = attributes.fetch(:settings_file, '')
		report = attributes.fetch(:report, nil)
		reportXsl = attributes.fetch(:report_xsl, '')
		failOnError = attributes.fetch(:fail_on_error, true)
		
		return if directories.empty? and files.empty?
		
		FileUtils.mkdir_p report.dirname
		
		stylecop = tool.to_absolute
		
		sh "#{stylecop.escape} --configurationSymbols CODE_ANALYSIS #{"--directories #{directories.map do |d| d.to_absolute.escape + " " end}" if not directories.empty?} #{"--files #{files.map do |f| f.to_absolute.escape + " " end}" if not files.empty?} #{"--ignoreFilePattern #{ignoreFilePattern.map do |p| p.escape + " " end}" if not ignoreFilePattern.empty?} #{'--recurse' if recurse} #{"--styleCopSettingsFile #{settingsFile.to_absolute.escape}" if settingsFile} #{"--xslFile #{reportXsl.to_absolute.escape}" if reportXsl} #{"--outputXmlFile #{report.to_absolute.escape}" if report}" \
		do |ok, status|
			raise "StyleCop could not be run" if not ok
			
			violationsFile = report.ext('violations.xml').to_absolute
			
			doc = Document.new(File.read(violationsFile))
			violations = XPath.first doc, 'count(//StyleCopViolations/Violation)' || 0
			
			yield violations if block_given?
			
			raise "StyleCop reported #{violations} violation(s), see #{report}" if violations > 0 and failOnError
		end
	end
end