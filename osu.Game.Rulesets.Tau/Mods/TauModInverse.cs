﻿using System;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModInverse : Mod, IApplicableToDrawableHitObject, IApplicableToDrawableRuleset<TauHitObject>
    {
        public override string Name => "Inverse";
        public override string Acronym => "IN";
        public override ModType Type => ModType.DifficultyIncrease;
        public override string Description => @"Hit objects will come outside of the playfield.";
        public override double ScoreMultiplier => 1.2;
        public override Type[] IncompatibleMods => new[] { typeof(TauModHidden), typeof(TauModFadeIn) };

        public void ApplyToDrawableHitObject(DrawableHitObject drawable)
        {
            drawable.ApplyCustomUpdateState += (drawableObject, state) =>
            {
                switch (drawableObject)
                {
                    case DrawableSliderHead head:
                        applyInverseToBeat(head, -0.484f, -0.984f);

                        break;

                    case DrawableBeat beat:
                        applyInverseToBeat(beat, -0.516f);

                        break;

                    case DrawableHardBeat hardBeat:
                        var hardBeatObject = hardBeat.HitObject;

                        hardBeat.ClearTransforms();

                        using (hardBeat.BeginAbsoluteSequence(hardBeatObject.StartTime - hardBeatObject.TimePreempt))
                        {
                            hardBeat.ResizeTo(2);
                            hardBeat.ResizeTo(1, hardBeatObject.TimePreempt);
                        }

                        break;

                    case DrawableSlider slider:
                        slider.ApplyInverseChanges();

                        break;
                }
            };
        }

        private void applyInverseToBeat(DrawableBeat beat, float finalDistance, float startingDistance = -1)
        {
            var box = beat.Box;
            var hitObject = beat.HitObject;

            box.ClearTransforms(targetMember: "Y");

            using (beat.BeginAbsoluteSequence(hitObject.StartTime, false))
            {
                box.MoveToY(startingDistance);
                box.MoveToY(finalDistance, hitObject.TimePreempt);
            }
        }

        public void ApplyToDrawableRuleset(DrawableRuleset<TauHitObject> drawableRuleset)
        {
            var playfield = (TauPlayfield)drawableRuleset.Playfield;
            playfield.Inversed = true;

            // This is to make Inverse more enjoyable to play, without tweaking everything to accommodate a smaller playfield.
            playfield.Scale = new Vector2(0.75f);
        }
    }
}
