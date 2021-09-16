require 'xcodeproj'

project_path = ARGV[0]
project = Xcodeproj::Project.open(project_path)

project.targets.each do |target|
  target.build_configurations.each do |config|
    config.build_settings['ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES'] = target.name == 'Unity-iPhone' ? 'YES' : 'NO'
  end


end

project.save(nil)