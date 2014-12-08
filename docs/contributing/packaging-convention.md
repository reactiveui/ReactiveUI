
ghuntley [8:28 PM] So for community adding to RxUI was thinking of a single repo under reactiveui/community and creating convention of ReactiveUICommunity.ComponentName

ReactiveUICommunity.NetworkConnectivityService.Platform

ghuntley [8:30 PM]
ReactiveUICommunity.DeviceInformationService.Platform

ghuntley [8:31 PM]
It creates a deliberate line of responsibility; community is community supported and -core is done by team and community. Thoughts?

ghuntley [8:35 PM]
NuGet convention of ReactiveUICommunity-PackageName and classify ReactiveUI-* reserved for -core

ghuntley [10:00 PM] 
Who would be the nuget owner of the packages though. Needs to be centralised potentially to prevent AWOL maintainers.

michaelteper [3:15 AM] 
Are there community projects out there already, or is this putting the cart before the horse?

paulcbetts [3:19 AM] 
I'm not super excited about the "Community" trope, it was mostly created by Microsoft employees so people would stop complaining about closed source MS components

paulcbetts [3:20 AM]
We should just take PRs instead

paulcbetts [3:20 AM]
And have a list of "Libraries that go great with RxUI"

moswald [3:56 AM] 
agreed

ghuntley [5:06 AM] 
@michaelteper:  cart before horse; just getting sick of copy and pasta of the same network service between solutions and the though is that it's not big enough for its own custom name. Thus it would be good to come up with convention what is in -core namespace and what is not. Also stance on namespace protection generally needed- do we send polite emails to library maintainers that pollute the -core namespace?

ghuntley [5:14 AM] 
@paulcbetts ServiceStack actively does the polite to ensure that no confusion/pollution/doesn't get issues on -core for 3rd party plugins. Ie If you use the ServiceStack.* namespace or as prefix to a nuget package you get a polite email from Dennis. Not advocating this route; sussing out feel.

ghuntley [5:16 AM]
MVX has the MvxPlugins prefix that the community has somewhat unofficially adopted but it's pretty fragmented. PersonName.PluginName or CompanyName.Plugin or just PluginName; get enough plugins and your "using a" start to look like a wasteland

paulcbetts [11:35 AM] 
These are good things to think about, but I don't think it's been a problem so far; we should encourage people to make their own NuGet packages though, then make sure to do some marketing for them

paulcbetts [11:35 AM]
Since we haven't really encouraged people to camp on our namespaces so far like Mvx does, we don't really have this problem