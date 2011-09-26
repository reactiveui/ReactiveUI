require 'albacore'
require 'fileutils'

#
# COOLSTUFF: Rake tasks to automate common tasks
#
# If you install Ruby (via http://ruby-lang.org), as well as the Albacore gem
# (gem install albacore), Kitchen Sink comes with an automated build and
# packaging system, which is really easy to extend.
#
# Some Interesting Commands:
#
# rake build    # Builds all of your solutions, and places files in ./bin
#
# rake publish  # Publishes your app using ClickOnce, and zips up a copy of the
#               # binaries in the same folder
#

###
### Project Settings
###

HolyCrapDidIActuallyReadThisSection = true

ProductInformation = {
    :product_name => "Reactive XAML",
    :version => "1.1.1.0",
    :company_name => "Paul Betts",
    :description => "An MVVM framework that integrates the Reactive Extensions",
}

CustomPrebuildTasks = {
    :reactivexaml => [:assemblyinfo_reactivexaml, :assemblyinfo_reactivexamlblend],
}


###
### Custom Tasks
###

desc "Generate an AssemblyInfo.cs for ReactiveUI"
assemblyinfo :assemblyinfo_reactivexaml do |asm|
    ProductInformation.each {|k,v| set_property(asm, k, v)}
    asm.output_file = "ReactiveUI/Properties/AssemblyInfo.cs"
end

desc "Generate an AssemblyInfo.cs for ReactiveUI.Blend"
assemblyinfo :assemblyinfo_reactivexamlblend do |asm|
    ProductInformation.each {|k,v| set_property(asm, k, v)}
    asm.product_name = "Reactive XAML Expression Blend"
    asm.output_file = "ReactiveUI.Blend/Properties/AssemblyInfo.cs"
end





###############################################################################
##
## STOP READING NOW, you don't *have* to edit this section, unless you want to
## of course.
##
###############################################################################

###
### Constants and functions that don't need to be changed per project
###

RootProjectDir = File.dirname(__FILE__)

def sln_file_to_name(path, prefix)
    an_underscore = prefix ? '_' : ''
    fancy_name = [prefix, an_underscore, File.basename(path, '.sln').gsub(/([-_]|\.)/, '')].join('').downcase
    fancy_name.to_sym
end

def set_property(object, property, value)
    object.send((property.to_s + '=').to_sym, value)
end

def publish_dir_for_sln(sln_file)
    "#{ClickOnceRootPublishLocation}/#{sln_file_to_name(sln_file, nil)}"
end

@all_solution_files = Dir.glob(File.join(RootProjectDir, '**', '*.sln'))

###
### Generated Tasks
###

@all_solution_files.each do |sln_file|
    clean_task_names = ["clean_dbg", "clean_rel"].map{|x| sln_file_to_name(sln_file,x)}

    desc "Cleans '#{sln_file}'"
    task sln_file_to_name(sln_file, "clean") => clean_task_names

    clean_task_names.zip([:debug, :release]).each do |x|
        msbuild x[0] do |msb|
            msb.solution = sln_file
            msb.properties = {:configuration => x[1]}
            msb.targets [:clean]
        end
    end

    custom_prebuild_tasks = CustomPrebuildTasks[sln_file_to_name(sln_file, nil)] || []
    desc "Builds '#{sln_file}'"
    msbuild sln_file_to_name(sln_file, "build") => custom_prebuild_tasks do |msb|
        msb.solution = sln_file
        msb.properties = {
            :configuration => :release,
            :applicationversion => ProductInformation[:version],
        }

        msb.targets [:build]
    end

    clean_and_build = ["clean", "build"].map {|x| sln_file_to_name(sln_file, x)}
    desc "Publishes '#{sln_file}'"
    msbuild sln_file_to_name(sln_file, "publish") => clean_and_build do |msb|
        # NB: These paths have to end in a slash or else MSBuild loses it
        publish_dir = publish_dir_for_sln(sln_file) + '/'

        msb.solution = sln_file

        msb.properties = {
            :configuration => :release,
            :publishdir => publish_dir,
            :publishurl => publish_dir,
            :installurl => publish_dir,
            :applicationversion => ProductInformation[:version]
        }

        msb.targets [:publish]
    end
end

@bin_dir = File.join(RootProjectDir, 'bin')

desc "Clears out the 'bin' directory"
task :blow_away_bin_folder do
    FileUtils.rm_rf @bin_dir
end

desc "Clears out the TestResults directory"
task :blow_away_test_results do
    FileUtils.rm_rf File.join(RootProjectDir, 'TestResults')
end

desc "Copies binaries from all of the subprojects to one 'bin' folder"
task :copy_binaries_to_bin_folder do
    Dir.glob("**/bin/Release").each do |release_dir|
        `robocopy "#{release_dir}" "#{@bin_dir}" /MIR`
    end
end

desc "Publishes a zipped archive of the binaries for a particular release"
zip :publish_archive do |z|
    z.directories_to_zip 'bin'
    z.output_file = "Binaries-#{ProductInformation[:version]}-#{Time.now.strftime('%Y%m%d-%H%M%S')}.zip"
    z.output_path = File.join(RootProjectDir, '..')
end

desc "Publishes a zipped archive of the source files"
zip :source_zip => :clean do |z|
    z.directories_to_zip RootProjectDir
    z.output_file = "#{File.basename(RootProjectDir)}-#{ProductInformation[:version]}-#{Time.now.strftime('%Y%m%d-%H%M%S')}.zip"
    z.output_path = File.join(RootProjectDir, '..')
end



###
### Generate the "all" tasks
###

desc "Cleans all projects"
task :clean => @all_solution_files.map{|f| sln_file_to_name(f, "clean")} + [:blow_away_bin_folder, :blow_away_test_results]

desc "Builds all projects"
task :build => @all_solution_files.map{|f| sln_file_to_name(f, "build")} + [:copy_binaries_to_bin_folder]

desc "Publishes all projects"
task :publish => @all_solution_files.map{|f| sln_file_to_name(f, "publish")} + [:copy_binaries_to_bin_folder, :publish_archive]

task :default => [:build]


# Punish those who don't read the README

unless HolyCrapDidIActuallyReadThisSection
    STDERR.puts <<-EOS

        *************************************************************************
        **                                                                     **
        ** Hey! You need to edit Rakefile before you can actually use this, or **
        ** else weird things will happen.                                      **
        **                                                                     **
        *************************************************************************
    EOS
    exit 1
end
