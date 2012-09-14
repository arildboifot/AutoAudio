using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoAudio.Impl;
using NUnit.Framework;

namespace AutoAudio.Tests
{
    public class SoundConfigTests
    {
        [Test]
        public void ParsesOutputCorrectly()
        {
            string output = "SoundSwitch Audio Interface Changer\n0: Digital Audio (S/PDIF) (5- High Definition Audio Device)\n1: Digital Audio (S/PDIF) (5- High Definition Audio Device)\n2: Speakers (2- Logitech G930 Headset)\n3: Speakers (5- High Definition Audio Device) (default)\n4: Headphones (5- High Definition Audio Device)\n";

            var devices = SoundConfig.ParseEndpointOutput(output).ToList();

            Assert.AreEqual(5, devices.Count);
            Assert.True(devices[3].IsDefault);
        }
    }
}
