// Copyright 2020-2021 CesiumGS, Inc. and Contributors

using UnrealBuildTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class CesiumRuntime : ModuleRules
{
    public CesiumRuntime(ReadOnlyTargetRules Target) : base(Target)
    {
        PCHUsage = ModuleRules.PCHUsageMode.UseExplicitOrSharedPCHs;

        PublicIncludePaths.AddRange(
            new string[] {
                Path.Combine(ModuleDirectory, "../ThirdParty/include")
            }
        );


        PrivateIncludePaths.AddRange(
            new string[] {
                // ... add other private include paths required here ...
            }
        );

        string libPrefix;
        string libPostfix;
        string platform;
        if (Target.Platform == UnrealTargetPlatform.Win64) {
            platform = "Windows-x64";
            libPostfix = ".lib";
            libPrefix = "";
        }
        else if (Target.Platform == UnrealTargetPlatform.Mac) {
            platform = "Darwin-x64";
            libPostfix = ".a";
            libPrefix = "lib";
        }
        else if(Target.Platform == UnrealTargetPlatform.Linux) {
            platform = "Linux-x64";
            libPostfix = ".a";
            libPrefix = "lib";
        }
        else {
            platform = "Unknown";
            libPostfix = ".Unknown";
            libPrefix = "Unknown";
        }

        string libPath = Path.Combine(ModuleDirectory, "../ThirdParty/lib/" + platform);

        string releasePostfix = "";
        string debugPostfix = "d";

        bool preferDebug = (Target.Configuration == UnrealTargetConfiguration.Debug || Target.Configuration == UnrealTargetConfiguration.DebugGame);
        string postfix = preferDebug ? debugPostfix : releasePostfix;

        string[] libs = new string[]
        {
            "async++",
            "Cesium3DTiles",
            "CesiumAsync",
            "CesiumGeometry",
            "CesiumGeospatial",
            "CesiumGltfReader",
            "CesiumGltf",
            "CesiumJsonReader",
            "CesiumUtility",
            "draco",
            "modp_b64",
            "spdlog",
            "sqlite3",
            "tinyxml2",
            "uriparser"
        };

        if (preferDebug)
        {
            // We prefer Debug, but might still use Release if that's all that's available.
            foreach (string lib in libs)
            {
                string debugPath = Path.Combine(libPath, libPrefix + lib + debugPostfix + libPostfix);
                if (!File.Exists(debugPath))
                {
                    Console.WriteLine("Using release build of cesium-native because a debug build is not available.");
                    preferDebug = false;
                    postfix = releasePostfix;
                    break;
                }
            }
        }

        PublicAdditionalLibraries.AddRange(libs.Select(lib => Path.Combine(libPath, libPrefix + lib + postfix + libPostfix)));

        PublicDependencyModuleNames.AddRange(
            new string[]
            {
                "Core",
                // ... add other public dependencies that you statically link with here ...
            }
        );


        PrivateDependencyModuleNames.AddRange(
            new string[]
            {
                "RHI",
                "CoreUObject",
                "Engine",
                "MeshDescription",
                "StaticMeshDescription",
                "HTTP",
                "MikkTSpace",
                "LevelSequence",
                "Projects"
            }
        );

        PublicDefinitions.AddRange(
            new string[]
            {
                "SPDLOG_COMPILED_LIB",
                "LIBASYNC_STATIC"
            }
        );

        if (Target.bCompilePhysX && !Target.bUseChaos)
        {
            PrivateDependencyModuleNames.Add("PhysXCooking");
            PrivateDependencyModuleNames.Add("PhysicsCore");
        }
        else
        {
            PrivateDependencyModuleNames.Add("Chaos");
        }

        if (Target.bBuildEditor == true)
        {
            PublicDependencyModuleNames.AddRange(
                new string[] {
                    "UnrealEd",
                    "Slate",
                    "SlateCore",
                }
            );
        }

        DynamicallyLoadedModuleNames.AddRange(
            new string[]
            {
                // ... add any modules that your module loads dynamically here ...
            }
        );

        PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;
        PrivatePCHHeaderFile = "Private/PCH.h";
        CppStandard = CppStandardVersion.Cpp17;
        bEnableExceptions = true;
    }
}
