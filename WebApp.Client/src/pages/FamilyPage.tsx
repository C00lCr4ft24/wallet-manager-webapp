import FamilyInviteList from "@/components/FamilyInviteList"
import { FamilyUserList } from "@/components/FamilyUserList"
import InviteFamilyUserDialog from "@/components/InviteFamilyUserDialog"
import { useState } from "react"

function FamilyPage() {
  const [refetchTrigger, setRefetchTrigger] = useState<number>(0)

  return (
    <div className="px-12 py-12 flex h-screen">
      <div className="w-1/2 pr-12">
        <div className="flex items-center justify-between mb-6">
          <h1 className="text-3xl font-bold">Family Members</h1>
        </div>
        <FamilyUserList refetchTrigger={refetchTrigger} />
      </div>
      <div className="w-1/2 pl-12 border-l">
        <div className="flex items-center justify-between mb-6">
          <h1 className="text-3xl font-bold">Sent Invites</h1>
          <InviteFamilyUserDialog onInviteSent={() => setRefetchTrigger(prev => prev + 1)} />
        </div>
        <FamilyInviteList loadType="sent" refetchTrigger={refetchTrigger} onAction={() => setRefetchTrigger(prev => prev + 1)} />
        <div className="flex items-center justify-between mb-6 mt-12">
          <h1 className="text-3xl font-bold">Received Invites</h1>
        </div>
        <FamilyInviteList loadType="received" refetchTrigger={refetchTrigger} onAction={() => setRefetchTrigger(prev => prev + 1)} />
      </div>
    </div>
  )
}

export default FamilyPage