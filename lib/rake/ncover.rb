require 'rexml/document'
#require 'win32/registry'
include REXML
#include Win32

class NCover
	def self.run_coverage(attributes)
		tool = attributes.fetch(:tool)
		program = attributes.fetch(:program).to_absolute
		assembly = attributes.fetch(:assembly)
		args = attributes.fetch(:args, [])
		reportDirectory = attributes.fetch(:report_dir, '.')
		workingDirectory = attributes.fetch(:working_dir, '.').to_absolute
		applicationAssemblies = attributes.fetch(:application_assemblies)
		#registerProfiler = attributes.fetch(:register_profiler, true)

		xmlReport = assembly.name.ext('Coverage.xml').in(reportDirectory).to_absolute
		logFile = assembly.name.ext('Coverage.log').in(reportDirectory).to_absolute
		FileUtils.mkdir_p xmlReport.dirname
		FileUtils.mkdir_p logFile.dirname 
		
		ncover = tool.to_absolute

		#register registerProfiler, tool do
			sh "#{ncover.escape} #{program.escape} #{assembly} #{args.join(' ')} //a #{applicationAssemblies} //w #{workingDirectory.escape} //x #{xmlReport.escape} //l #{logFile.escape} //reg //v"
		#end
	end
	
#	def self.register(registerProfiler, tool)
#		begin
#			if registerProfiler
#				Registry::HKEY_CURRENT_USER.create('Software\KiwiNova\NCoverExplorer', Registry::KEY_WRITE | Registry::KEY_READ) do |reg|
#					begin
#						refCount = reg.read_i 'NCoverRefCount'
#					rescue
#						refCount = 0
#					end 
#					
#					reg.write_i 'NCoverRefCount', refCount + 1
#				end
#				
#				Registry::HKEY_CURRENT_USER.create('Software\Classes\CLSID\{6287B5F9-08A1-45e7-9498-B5B2E7B02995}', Registry::KEY_WRITE) do |reg|
#					reg.write_s nil, 'NCover Profiler Object'
#					
#					reg.create('InprocServer32', Registry::KEY_WRITE) do |subkey|
#						subkey.write_s nil, 'CoverLib.dll'.in(tool.dirname).to_absolute
#						subkey.write_s 'ThreadingModel', 'Both'
#					end
#				end
#			end
#			
#			yield if block_given?
#		ensure
#			if registerProfiler
#				Registry::HKEY_CURRENT_USER.create('Software\KiwiNova\NCoverExplorer', Registry::KEY_WRITE | Registry::KEY_READ) do |reg|
#					begin
#						refCount = reg.read_i 'NCoverRefCount'
#					rescue
#						refCount = 1
#					end 
#					
#					refCount -= 1
#					
#					if refCount == 0
#						Registry::HKEY_CURRENT_USER.delete_key 'Software\KiwiNova\NCoverExplorer', true
#						Registry::HKEY_CURRENT_USER.delete_key 'Software\Classes\CLSID\{6287B5F9-08A1-45e7-9498-B5B2E7B02995}', true
#					else
#						reg.write_i 'NCoverRefCount', refCount if refCount >= 1
#					end
#				end
#			end
#		end
#	end

	def self.explore(attributes)
		tool = attributes.fetch(:tool)
		project = attributes.fetch(:project)
		reportDirectory = attributes.fetch(:report_dir, '.')
		minCoverage = attributes.fetch(:min_coverage, 90)
		fail = attributes.fetch(:fail_if_under_min_coverage, false)
		htmlReport = attributes.fetch(:html_report, 'Coverage.html').in(reportDirectory).to_absolute
		xmlReport = attributes.fetch(:xml_report, 'Coverage.xml').in(reportDirectory).to_absolute
		statistics = attributes.fetch(:statistics, {})
		
		files = FileList.new("#{reportDirectory}/*.Coverage.xml").map! do |file|
			file.to_absolute.escape
		end
		
		return if files.empty?
		
		FileUtils.mkdir_p htmlReport.dirname
		FileUtils.mkdir_p xmlReport.dirname 
		
		ncoverExplorer = tool.to_absolute
			
		sh "#{ncoverExplorer.escape} #{files} /report:ModuleClassFunctionSummary /sort:Name #{'/failCombinedMinimum' if fail} /minCoverage:#{minCoverage} /project:#{project.escape} /html:#{htmlReport.escape} /xml:#{xmlReport.escape}" \
		do |ok, status| 
			if ok or status.exitstatus == 3
				doc = Document.new(File.read(xmlReport))
				
				statistics.each do |key, query|
					value = XPath.first(doc, query)
					
					yield key, value if block_given?
					
					statistics[key] = value
				end
			end

			TeamCity.append_build_status_text "Code coverage is below accepted level of #{minCoverage}%" if status.exitstatus == 3
			
			raise "NCoverExplorer failed" if not ok or (fail and status.exitstatus == 3)
		end
		
		return statistics
	end
end
