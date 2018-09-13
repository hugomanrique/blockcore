﻿using System;
using System.Runtime.CompilerServices;
using Stratis.ModuleValidation.Net.Determinism;
using Stratis.ModuleValidation.Net.Format;
using Stratis.SmartContracts.Core.Validation.Policy;

namespace Stratis.SmartContracts.Core.Validation
{
    /// <summary>
    /// Defines a policy used to evaluate a TypeDefinition for uses of non-deterministic Types
    /// </summary>
    public static class DeterminismPolicy
    {
        public static WhitelistPolicy WhitelistPolicy = new WhitelistPolicy()
            .Namespace(nameof(System), AccessPolicy.Denied, SystemPolicy)
            .Namespace(typeof(RuntimeHelpers).Namespace, AccessPolicy.Denied, CompilerServicesPolicy)
            .Namespace(typeof(SmartContract).Namespace, AccessPolicy.Allowed, SmartContractsPolicy);

        public static ValidationPolicy Default = new ValidationPolicy()
            .WhitelistValidator(WhitelistPolicy)
            .TypeDefValidator(new FinalizerValidator())
            .InstructionValidator(new FloatValidator());

        private static void SystemPolicy(NamespacePolicy policy)
        {
            foreach (Type type in Primitives.Types)
            {
                policy
                    .Type(type, AccessPolicy.Allowed);
            }

            policy
                .Type(typeof(void).Name, AccessPolicy.Allowed)
                .Type(typeof(object).Name, AccessPolicy.Denied, 
                    m => m.Member(nameof(ToString), AccessPolicy.Allowed)
                          .Constructor(AccessPolicy.Allowed));
        }

        private static void CompilerServicesPolicy(NamespacePolicy compilerServices)
        {
            compilerServices
                .Type(nameof(IteratorStateMachineAttribute), AccessPolicy.Allowed)
                .Type(nameof(RuntimeHelpers), AccessPolicy.Denied,
                    t => t.Member(nameof(RuntimeHelpers.InitializeArray), AccessPolicy.Allowed)
                );
        }

        private static void SmartContractsPolicy(NamespacePolicy policy)
        {
            policy
                .Type(typeof(SmartContract), AccessPolicy.Allowed);
        }
    }
}