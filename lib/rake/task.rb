class Rake::Task
	old_execute = self.instance_method(:execute)
	
	define_method(:execute) do |args|
		puts "\n[#{name}]\n"
		old_execute.bind(self).call(args)
	end
end