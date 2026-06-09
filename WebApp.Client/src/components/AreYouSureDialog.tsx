import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { useState } from "react"

function AreYouSureDialog({ children, title = "Are you sure?", description = "This action cannot be undone.", confirmTitle, cancelTitle, onConfirm, onCancel }: { children: React.ReactNode, title: string, description: string, confirmTitle: string, cancelTitle: string, onConfirm: () => void, onCancel: () => void }) {
  const [isOpen, setIsOpen] = useState<boolean>(false)

  return (
    <Dialog open={isOpen} onOpenChange={setIsOpen}>
      <form>
        <DialogTrigger asChild>
          {children}
        </DialogTrigger>
        <DialogContent className="sm:max-w-sm">
          <DialogHeader>
            <DialogTitle>{title}</DialogTitle>
            <DialogDescription>
              {description}
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <DialogClose asChild>
              <Button variant="outline" onClick={onCancel}>
                {cancelTitle}
              </Button>
            </DialogClose>
            <Button type="submit" onClick={() => { onConfirm(); setIsOpen(false); }}>
              {confirmTitle}
            </Button>
          </DialogFooter>
        </DialogContent>
      </form>
    </Dialog>
  )
}

export default AreYouSureDialog