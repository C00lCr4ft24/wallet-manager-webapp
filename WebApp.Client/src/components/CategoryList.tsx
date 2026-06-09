import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"

import { useEffect, useState } from "react"
import { toast } from "sonner"
import { CategoryHeaderDto, ProblemDetails } from "@/generated/dtos"
import { Button } from "./ui/button"
import categoryService from "@/services/categoryService"
import { Avatar } from "primereact/avatar"
import CategoryDialog from "./CategoryDialog"

function CategoryList({ refetchTrigger }: { refetchTrigger?: number }) {
  const [categories, setCategories] = useState<CategoryHeaderDto[]>([])
  const [isLoading, setIsLoading] = useState<boolean>(true)

  const [editTrigger, setEditTrigger] = useState<number>(0)

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const data = await categoryService.GetCategories()
        setCategories(data)
      } catch (e) {
        if (e instanceof ProblemDetails) {
          toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
        }
      } finally {
        setIsLoading(false)
      }
    }
    fetchCategories()
  }, [refetchTrigger, editTrigger])

  const numOfColumns = 6

  return (
    <div className="w-full max-w-6xl mx-auto font-mono">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead colSpan={1}>Icon</TableHead>
            <TableHead colSpan={1}>Name</TableHead>
            <TableHead colSpan={1}>Description</TableHead>
            <TableHead className="text-right" colSpan={1}>
              Actions
            </TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {isLoading ? (
            <TableRow>
              <TableCell colSpan={numOfColumns} className="text-center">
                Loading...
              </TableCell>
            </TableRow>
          ) : categories.length === 0 ? (
            <TableRow>
              <TableCell colSpan={numOfColumns} className="text-center">
                No categories found.
              </TableCell>
            </TableRow>
          ) : (
            categories.map((category) => (
              <TableRow key={category.id} style={{ backgroundColor: category.color ? category.color + '50' : 'transparent' }}>
                <TableCell className="w-10">
                  <Avatar size="large" label={category.icon} shape="circle" style={{ backgroundColor: category.color ?? 'transparent' }} />
                </TableCell>
                <TableCell>{category.name}</TableCell>
                <TableCell>{category.description}</TableCell>
                <TableCell className="text-right">
                  {!category.isDefault &&
                    < CategoryDialog category={category} onChanges={() => { setEditTrigger(prev => prev + 1) }}>
                      <Button variant="link">
                        Edit
                      </Button>
                    </CategoryDialog>
                  }
                </TableCell>
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>
    </div >
  )
}

export default CategoryList