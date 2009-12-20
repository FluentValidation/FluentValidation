class String
	def escape
		"\"#{self.to_s}\""
	end
	
	def in(dir)
		File.join(dir, self)
	end
	
	def name()
		File.basename(self)
	end	
	
	def dirname()
		File.dirname(self)
	end

	def to_absolute()
		File.expand_path(self)
	end
end