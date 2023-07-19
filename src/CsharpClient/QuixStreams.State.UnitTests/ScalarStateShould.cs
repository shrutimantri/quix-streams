﻿using System;
using FluentAssertions;
using QuixStreams.State.Storage;
using Xunit;

namespace QuixStreams.State.UnitTests
{
    public class ScalarStateShould
    {
        
        [Fact]
        public void Constructor_UsingNonEmptyState_ShouldLoadState()
        {
            // Arrange
            var storage = new InMemoryStorage();
            storage.Set(ScalarState.StorageKey, new StateValue("whatever"));
            var state = new ScalarState(storage);

            // Assert
            state.Value.StringValue.Should().BeEquivalentTo("whatever");
        }
        
        [Fact]
        public void SetValue_ShouldChangeValue()
        {
            // Arrange
            var storage = new InMemoryStorage();
            var state = new ScalarState(storage);

            // Act
            state.Value = new StateValue("value");

            // Assert
            state.Value.StringValue.Should().BeEquivalentTo("value");
        }

        [Fact]
        public void Clear_Value_ShouldSetToNull()
        {
            // Arrange
            var storage = new InMemoryStorage();
            var state = new ScalarState(storage);
            state.Value = new StateValue("value");

            // Act
            state.Clear();

            // Assert
            state.Value.Should().BeNull();
        }

        [Fact]
        public void Flush_Value_ShouldPersistChangesToStorage()
        {
            // Arrange
            var storage = new InMemoryStorage();
            var state = new ScalarState(storage);
            state.Value = new StateValue("value");

            // Act
            state.Flush();

            // Assert
            storage.Get(ScalarState.StorageKey).StringValue.Should().BeEquivalentTo("value");
        }
        
        [Fact]
        public void Flush_ClearBeforeFlush_ShouldClearStorage()
        {
            // Arrange
            var storage = new InMemoryStorage();
            var state = new ScalarState(storage);
            state.Value = new StateValue("value");
            state.Flush();
            state.Clear();

            // Act
            state.Flush();

            // Assert
            storage.ContainsKey(ScalarState.StorageKey).Should().BeFalse();
        }

        [Fact]
        public void State_WithNullStorage_ShouldThrowArgumentNullException()
        {
            // Act
            Action act = () => new ScalarState(null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Reset_Modified_ShouldResetToSaved()
        {
            // Arrange
            var storage = new InMemoryStorage();
            var state = new ScalarState(storage);
            state.Value = new StateValue("value");
            state.Flush();

            // Act
            state.Value = new StateValue("updatedValue");
            state.Reset();

            // Assert
            state.Value.StringValue.Should().BeEquivalentTo("value");
        }
        
        [Fact]
        public void Update_WithNullByteValue_ShouldBeRemovedFromState()
        {
            // Arrange
            var storage = new InMemoryStorage();
            var state = new ScalarState(storage);
            state.Value = new StateValue(new byte[] {1,2,3});
            state.Flush();
            storage.ContainsKey(ScalarState.StorageKey).Should().BeTrue();

            // Act
            state.Value = new StateValue((byte[])null);
            state.Flush();

            // Assert
            storage.ContainsKey(ScalarState.StorageKey).Should().BeFalse();
        }
        
        [Fact]
        public void Update_WithNullObjectValue_ShouldBeRemovedFromState()
        {
            // Arrange
            var storage = new InMemoryStorage();
            var state = new ScalarState(storage);
            state.Value = new StateValue(new byte[] {1,2,3}, StateValue.StateType.Object);
            state.Flush();
            storage.ContainsKey(ScalarState.StorageKey).Should().BeTrue();

            // Act
            state.Value = new StateValue(null, StateValue.StateType.Object);
            state.Flush();

            // Assert
            storage.ContainsKey(ScalarState.StorageKey).Should().BeFalse();
        }
        
        [Fact]
        public void Update_WithNullStringValue_ShouldBeRemovedFromState()
        {
            // Arrange
            var storage = new InMemoryStorage();
            var state = new ScalarState(storage);
            state.Value = new StateValue("something");
            state.Flush();
            storage.ContainsKey(ScalarState.StorageKey).Should().BeTrue();

            // Act
            state.Value = new StateValue((string)null);
            state.Flush();

            // Assert
            storage.ContainsKey(ScalarState.StorageKey).Should().BeFalse();
        }
    }
}
