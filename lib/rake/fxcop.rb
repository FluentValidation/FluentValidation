require 'rexml/document'
include REXML

class FxCop
	def self.analyze(attributes)
		tool = attributes.fetch(:tool)
		project = attributes.fetch(:project).to_absolute
		report = attributes.fetch(:report).to_absolute
		applyReportXsl = attributes.fetch(:apply_report_xsl, false)
		reportXsl = attributes.fetch(:report_xsl, '.').to_absolute
		includeSummary = attributes.fetch(:include_summary, false)
		consoleOutput = attributes.fetch(:console_output, false)
		consoleXsl = attributes.fetch(:console_xsl, '.').to_absolute
		failOnError = attributes.fetch(:fail_on_error, true)
		showSummary = attributes.fetch(:show_summary, false)
		ignoreGeneratedCode = attributes.fetch(:ignore_generated_code, true)
		assemblies = attributes.fetch(:assemblies)
		
		return if assemblies.empty?
		
		FileUtils.mkdir_p report.dirname
		
		fxcop = tool.to_absolute
		
		sh "#{fxcop.escape} #{"/project:#{project.escape}"} #{'/ignoregeneratedcode' if ignoreGeneratedCode} #{'/summary' if showSummary} #{"/out:#{report.escape}"} #{'/applyoutxsl' if applyReportXsl} #{"/outxsl:#{reportXsl.escape}" if applyReportXsl} #{'/console' if consoleOutput} #{"/consolexsl:#{consoleXsl.escape}" if consoleOutput} #{assemblies.map do |f| "/f:#{f.to_absolute.escape}" end } /successfile" \
		do |ok, status|
			raise "FxCop could not be run" if not ok
			
			violations = 0
			if not File.exist? '.lastcodeanalysissucceeded'.in(report.dirname)
				# Re-run silently to generate an XML report to query later.
				sh "#{fxcop.escape} /quiet #{"/project:#{project.escape}"} #{'/ignoregeneratedcode' if ignoreGeneratedCode} #{"/out:#{report.ext('xml').escape}"} #{assemblies.map do |f| "/f:#{f.to_absolute.escape}" end } /successfile" \
			
				TeamCity.import_data 'FxCop', report.ext('xml')
				
				doc = Document.new(File.read(report.ext('xml')))
				violations = XPath.first doc, 'count(.//Message)' || 0
			end
			
			yield violations if block_given?
			
			raise "FxCop reported #{violations} violation(s), see #{report}" if violations > 0 and failOnError
		end
	end
end