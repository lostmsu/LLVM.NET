#include "llvm/CodeGen/GCStrategy.h"
#include "llvm/CodeGen/GCMetadata.h"
#include "llvm/Support/Compiler.h"

#include "DllApi.h"

using namespace llvm;

namespace {
	struct ExternalGCInfo{
		unsigned NeededSafePoints; //< Bitmask of required safe points.
		int CustomReadBarriers;   //< Default is to insert loads.
		int CustomWriteBarriers;  //< Default is to insert stores.
		int CustomRoots;          //< Default is to pass through to backend.
		int CustomSafePoints;     //< Default is to use NeededSafePoints
		//  to find safe points.
		int InitRoots;            //< If set, roots are nulled during lowering.
		int UsesMetadata;         //< If set, backend must emit metadata tables.

		bool (*initializeCustomLowering)(Module &F);
		bool (*performCustomLowering)(Function &F);
		bool (*findCustomSafePoints)(GCFunctionInfo& FI, MachineFunction& MF);
	};

	class LLVM_LIBRARY_VISIBILITY ExternalGC: public GCStrategy{
	private:
		bool (*initializeCustomLoweringF)(Module &F);
		bool (*performCustomLoweringF)(Function &F);
		bool (*findCustomSafePointsF)(GCFunctionInfo& FI, MachineFunction& MF);
	public:
		ExternalGC(ExternalGCInfo info)
		{
			this->NeededSafePoints = info.NeededSafePoints;
			this->CustomReadBarriers = info.CustomReadBarriers;
			this->CustomWriteBarriers = info.CustomWriteBarriers;
			this->CustomRoots = info.CustomRoots;
			this->CustomSafePoints = info.CustomSafePoints;

			this->InitRoots = info.InitRoots;
			this->UsesMetadata = info.UsesMetadata;

			this->initializeCustomLoweringF = info.initializeCustomLowering;
			this->performCustomLoweringF = info.performCustomLowering;
			this->findCustomSafePointsF = info.findCustomSafePoints;
		}

		bool initializeCustomLowering(Module &F) override {
			return this->initializeCustomLoweringF(F);
		}
		bool performCustomLowering(Function &F) override {
			return this->performCustomLoweringF(F);
		}
		bool findCustomSafePoints(GCFunctionInfo& FI, MachineFunction& MF) override {
			return this->findCustomSafePointsF(FI, MF);
		}
	};

	LLVMDLL_FUNC(GCStrategy*) LLVMCreateExternalGC(ExternalGCInfo info){
		return new ExternalGC(info);
	}

	LLVMDLL_FUNC(GCRegistry::node*) LLVMRegisterExternalGC(
			const char* name, const char* descr, GCStrategy* (*ctor)()){
		GCRegistry::entry* gcentry = new GCRegistry::entry(name, descr, ctor);
		return new GCRegistry::node(*gcentry);
	}
}