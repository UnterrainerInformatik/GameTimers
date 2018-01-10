[![NuGet](https://img.shields.io/nuget/v/GameTimers.svg?maxAge=2592000)](https://www.nuget.org/packages/GameTimers/) [![NuGet](https://img.shields.io/nuget/dt/GameTimers.svg?maxAge=2592000)](https://www.nuget.org/packages/GameTimers/)
 [![license](https://img.shields.io/github/license/unterrainerinformatik/collisiongrid.svg?maxAge=2592000)](http://unlicense.org)  [![Twitter Follow](https://img.shields.io/twitter/follow/throbax.svg?style=social&label=Follow&maxAge=2592000)](https://twitter.com/throbax)  

# ![Icon](https://github.com/UnterrainerInformatik/GameTimers/raw/master/icon.png)GameTimers

When programming a game you'll depend on timers heavily. Whether you have a damage effect, that needs to be calculated every x seconds, or a visual special effect to be controlled, you'll steer all that by adding time to variable and checking it against a limit.
Like with every work that gets repetitive, you'll get sloppy and you'll make mistakes.
And it's precisely for this reason that I wrote these classes.

This is a PCL (portable code library) so you should be able to use it in any of your MG projects.

> **If you like this repo, please don't forget to star it.**
> **Thank you.**



## Description

It should have a constructor where you would have to declare the timeout in milliseconds you want to measure.
Then you would have to update it every game-cycle, giving it the current gameTime object. Its update method will do the appropriate math with the appropriate values hidden in the gameTime object and return true, if the timeout has been reached.
It has some convenience-properties, which you may use to determine the percentage of time passed, to determine the time that is still left, or to reset or change the interval itself.
You may use a defaulted field in the update method, if you wish to add the excess time, that was not used after "triggering" the timer, to the next run of the timer (in case you have some sort of a cyclic timer because otherwise you would just "lose" this excess time).

A timer exposes several events like 'Fire' and 'Fired' which are triggered the moment it fires and afterwards and 'Updating' and 'Updated' which are triggered on update, regardless if the timer fires or not.

## Examples

Here are some examples of its usage:

This one declares a timer using the builder-pattern and sets it active.

```c#
SpecialAbilityTimerSpawner = Timer.Builder(0f).isActive(true);
```

This one is declaring two timers and connects them so that, when the first one fires, the second one gets started and the first one is set to in-active. Normally single timers are connected to themselves.

```c#
AbilityTimerPodRacer = Timer.Builder(0f).isActive(false).Fired(EnablingAbilityPodRacer);
AbilityDurationPodRacer = Timer.Builder(0f).isActive(false).Fired(DisablingAbilityPodRacer);
AbilityTimerPodRacer.Connect(AbilityDurationPodRacer);
```

And, a bit more complicated, this one is used for our spider. It waits until the first timer (AbilityTimerSpider) fires, then its starting to fade-out until the FadeOutTimer fires, then it stays invulnerable until the DurationSpider timer fires. That one starts to fade-in the spider again until the FadeIn timer fires. They all, of course, are connected.

```c#
AbilityTimerSpider = Timer.Builder(0f).isActive(false).Fired(StartFadingOutSpider);
AbilityFadeOutSpider =
        Timer.Builder(0f)
                .isActive(false)
                .Fired(StartInvulnerabilitySpider)
                .Updating(UpdatingAbilityFadeOutSpider);
AbilityDurationSpider = Timer.Builder(0f).isActive(false).Fired(StartFadingInSpider);
AbilityFadeInSpider =
        Timer.Builder(0f)
                .isActive(false)
                .Fired(EndAbilitySpider)
                .Updating(UpdatingAbilityFadeInSpider);
AbilityTimerSpider.Connect(AbilityFadeOutSpider).Connect(AbilityDurationSpider)._
     Connect(AbilityFadeInSpider);
```

You may not only subscribe to the Fired-event, but you may ask the timer on update if it just fired, like that:

```c#
if (SpecialAbilityTimerSpawner.Update(gameTime))
{
        // special activity is ready to start...                        
        MonsterTypes monsterType =
            MonsterMachine.GetMonsterTypeFromDescriptor(monsterToSpawnDescriptor);
        if (monsterType != MonsterTypes.NONE)
        {…}
}             
```
