﻿using System;
using Xunit;
using Renamer.Services;
using Renamer.Services.Models;

namespace Renamer.Tests {
    public class StringExtensionTests {

        [Fact]
        public void IsAnArticle_IsThe_ReturnTrue() {
            string word = "The";
            bool expected = true;

            Assert.Equal(expected, word.IsAnArticle());
        }

        [Theory]
        [InlineData("A")]
        [InlineData("THE")]
        public void IsAnArticle_AreArticles_ReturnsTrue(string val) {
            Assert.True(val.IsAnArticle());
        }

        [Theory]
        [InlineData("Howdy")]
        public void IsAnArticle_AreNotArticles_ReturnsFalse(string val) {
            Assert.False(val.IsAnArticle());
        }

        [Fact]
        public void ReplaceSpaces_StringHasSpaces_ReturnsWithPeriods() {
            string title = "The Chilling Adventures of Sabrina";
            string expected = "The.Chilling.Adventures.of.Sabrina";

            Assert.Equal(expected, title.ReplaceSpaces());
        }
        [Fact]
        public void ReplaceSpaces_StringHasOneWord_ReturnsSameString() {
            string title = "Legion";
            string expected = "Legion";

            Assert.Equal(expected, title.ReplaceSpaces());
        }

        [Theory]
        [InlineData("The Chilling Adventures of Sabrina")]
        public void RemoveFirstWordArticlesFromTitle_ArticlePresent_ReturnsRestOfString(string title) {
            string expected = "Chilling Adventures of Sabrina";
            Assert.Equal(expected, title.RemoveFirstWordArticlesFromTitle());
        }

        [Theory]
        [InlineData("Russian Doll")]
        public void RemoveFirstWordArticlesFromTitle_ArticleNotPresent_ReturnsFullString(string title) {
            string expected = "Russian Doll";
            Assert.Equal(expected, title.RemoveFirstWordArticlesFromTitle());
        }

        [Theory]
        [InlineData(@"Chapter One: October Country")]
        public void ReplaceInvalidChars_HasColonSpace_ReturnsStringWithHyphen(string val) {
            string expected = "Chapter One-October Country";
            Assert.Equal(expected, val.ReplaceInvalidChars());
        }

        [Theory]
        [InlineData(@"Sh*t Show at the F**k Factory")]
        public void ReplaceInvalidChars_HasAsterisks_ReturnsStringWithHyphens(string val) {
            string expected = "Sh-t Show at the F--k Factory";
            Assert.Equal(expected, val.ReplaceInvalidChars());
        }

        [Theory]
        [InlineData(@"Succession - 1.06 - Which Side Are You On?")]
        public void ReplaceInvalidChars_HasQuestionMark_ReturnsStringWithoutQuestionMark(string val) {
            string expected = "Succession - 1.06 - Which Side Are You On";
            Assert.Equal(expected, val.ReplaceInvalidChars());
        }

        [Theory]
        [InlineData(@"Chapter Six/ An Exorcism in Greendale")]
        [InlineData(@"Chapter Six\ An Exorcism in Greendale")]
        public void ReplaceInvalidChars_HasInvalidChars_ReturnsStringWithHyphen(string val) {
            string expected = "Chapter Six- An Exorcism in Greendale";
            Assert.Equal(expected, val.ReplaceInvalidChars());
        }

        [Fact]
        public void GetLevenshteinDistance_DistanceOf1_Passes() {
            string stringToTest = "The Chilling Adventures of Sabrina";
            string stringToCompare = "the Chilling Adventures of Sabrina";
            int expected = 1;
            Assert.Equal(expected, stringToTest.GetLevenshteinDistance(stringToCompare));
        }

        [Fact]
        public void RemoveNonAlphanumeric_ContainsAlphaNumeric_Removes() {
            string stringToTest = "Marvel's Daredevil";
            string expected = "Marvels Daredevil";
            Assert.Equal(expected, stringToTest.RemoveNonAlphanumeric());
        }
    }
}
