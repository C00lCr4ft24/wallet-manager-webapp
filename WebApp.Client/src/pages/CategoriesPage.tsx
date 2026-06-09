import CategoryDialog from "@/components/CategoryDialog"
import CategoryList from "@/components/CategoryList"
import { Button } from "@/components/ui/button"
import { Plus } from "lucide-react"
import { useState } from "react"

function CategoriesPage() {
  const [refetchTrigger, setRefetchTrigger] = useState(0)

  return (
    <div className="px-12 py-12 w-full max-w-6xl mx-auto font-mono">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-3xl font-bold">Categories</h1>
        <CategoryDialog onChanges={() => setRefetchTrigger(prev => prev + 1)}>
          <Button><Plus />Add Category</Button>
        </CategoryDialog>
      </div>
      <CategoryList refetchTrigger={refetchTrigger} />
    </div>
  )
}

export default CategoriesPage