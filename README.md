#Justifications

Hello again,

So I will begin with explaining and justifying what i have done.

I began with the first task and made the changes we discussed in the interview (seen in the third commit)
I changed the linq query to be a left outer join. I also added in-memory tests for it by only creating a patient and no episode.

The second task I considered quite a bit. I decided to be bold and make several changes to reflect what i thought was a good approach given the time.

I changed the input to a string so that the majority of the inputs could got through the action. This allows me to create validation and specifally work out why an input is invalid.

I also changed the return type so that I could also communicate errors and successes together.
Had this been a live solution I would have taken a more measured approach to this.

With the third task, in the interest of speed i decided to inject a delegated handler into the httpMessage handlers. This will be called for every request and is an ideal place for me to capture all the necessary data. I simply write this information as serialised xml to a log file (a production solution would almost certainly go in to a db).

on reflection I could have spent a little more time to create a filter attribute so that I could apply the logging to where I like (action or controller).

I do hope this is close to what you were looking for and I hope you can understand they way I tackled the tasks given the time constraints.