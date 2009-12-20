class Tarantino
	def self.run(attributes)
		tool = attributes.fetch(:tool)
		action = camelize attributes.fetch(:action, 'Update').to_s, true
		scriptDirectory = attributes.fetch(:script_dir, '.').to_absolute
		server = attributes.fetch(:server, '.')
		database = attributes.fetch(:database)

		if not attributes.fetch(:sspi, false)
			security = "/Username:#{attributes.fetch(:username)} /Password:#{attributes.fetch(:password)}"
		else
			security = '/IntegratedAuthentication'
		end
		
		tarantino = tool.to_absolute
		
		sh "#{tarantino.escape} /Action:#{action} /ScriptDirectory:#{scriptDirectory.escape} /Server:#{server} /Database:#{database} #{security}"
	end
	
	# Borrowed from activesupport
	#
	# By default, +camelize+ converts strings to UpperCamelCase. If the argument to +camelize+
    # is set to <tt>:lower</tt> then +camelize+ produces lowerCamelCase.
    #
    # +camelize+ will also convert '/' to '::' which is useful for converting paths to namespaces.
    #
    # Examples:
    #   "active_record".camelize                # => "ActiveRecord"
    #   "active_record".camelize(:lower)        # => "activeRecord"
    #   "active_record/errors".camelize         # => "ActiveRecord::Errors"
    #   "active_record/errors".camelize(:lower) # => "activeRecord::Errors"
	def self.camelize(lower_case_and_underscored_word, first_letter_in_uppercase = true)
		if first_letter_in_uppercase
			lower_case_and_underscored_word.to_s.gsub(/\/(.?)/) { "::#{$1.upcase}" }.gsub(/(?:^|_)(.)/) { $1.upcase }
		else
			lower_case_and_underscored_word.first.downcase + camelize(lower_case_and_underscored_word)[1..-1]
		end
	end
end