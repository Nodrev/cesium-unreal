#include "Cesium3DTiles/BoundingVolume.h"
#include <algorithm>

namespace Cesium3DTiles {

    BoundingVolume transformBoundingVolume(const glm::dmat4x4& transform, const BoundingVolume& boundingVolume) {
        switch (boundingVolume.index()) {
        case 0:
        {
            const OrientedBoundingBox& boundingBox = std::get<OrientedBoundingBox>(boundingVolume);
            glm::dvec3 center = transform * glm::dvec4(boundingBox.getCenter(), 1.0);
            glm::dmat3 halfAxes = glm::dmat3(transform) * boundingBox.getHalfAxes();
            return OrientedBoundingBox(center, halfAxes);
        }
        case 1:
        {
            // Regions are not transformed.
            return boundingVolume;
        }
        case 2:
        {
            const BoundingSphere& boundingSphere = std::get<BoundingSphere>(boundingVolume);
            glm::dvec3 center = transform * glm::dvec4(boundingSphere.getCenter(), 1.0);

            double uniformScale = std::max(
                std::max(
                    glm::length(glm::dvec3(transform[0])),
                    glm::length(glm::dvec3(transform[1]))
                ),
                glm::length(glm::dvec3(transform[2]))
            );

            return BoundingSphere(center, boundingSphere.getRadius() * uniformScale);
        }
        default:
            return boundingVolume;
        }
    }

    glm::dvec3 getBoundingVolumeCenter(const BoundingVolume& boundingVolume) {
        switch (boundingVolume.index()) {
        case 0:
        {
            const OrientedBoundingBox& boundingBox = std::get<OrientedBoundingBox>(boundingVolume);
            return boundingBox.getCenter();
        }
        case 1:
        {
            const BoundingRegion& region = std::get<BoundingRegion>(boundingVolume);
            return region.getBoundingBox().getCenter();
        }
        case 2:
        {
            const BoundingSphere& boundingSphere = std::get<BoundingSphere>(boundingVolume);
            return boundingSphere.getCenter();
        }
        default:
            return glm::dvec3(0.0);
        }
    }

}