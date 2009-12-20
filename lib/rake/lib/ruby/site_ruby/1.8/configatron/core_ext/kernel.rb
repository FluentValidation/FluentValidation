module Kernel
  
  # Provides access to the Configatron storage system.
  def configatron
    Configatron.instance
  end
  
end