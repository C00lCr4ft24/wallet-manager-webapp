import LimitDialog from "@/components/LimitDialog"
import LimitList from "@/components/LimitList"
import { Button } from "@/components/ui/button"
import { Plus } from "lucide-react"
import { useState } from "react"


function LimitsPage() {
  const [refetchTrigger, setRefetchTrigger] = useState<number>(0)
  return (
    <div className="px-12 py-12 w-full max-w-6xl mx-auto font-mono">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-3xl font-bold">Limits</h1>
        <LimitDialog onChanges={() => setRefetchTrigger(prev => prev + 1)}>
          <Button><Plus />Add Limit</Button>
        </LimitDialog>
      </div>
      <LimitList refetchTrigger={refetchTrigger} />
    </div>
  )
}

export default LimitsPage