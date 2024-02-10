import "reflect-metadata"; // required
import {ICompositionalRoot} from "@setup/composition_root";
import {serviceContainer} from "@setup/inversify_config";
import {IocTypes} from "@setup/ioc_types";

(async (): Promise<void> => {
    const compositionalRoot = serviceContainer.get<ICompositionalRoot>(IocTypes.ICompositionalRoot);
    await compositionalRoot.start();
})();